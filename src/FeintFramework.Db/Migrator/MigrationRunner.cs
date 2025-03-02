using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using QuikGraph;
using QuikGraph.Algorithms;

namespace FeintFramework.Db.Migrator;

public class MigrationRunner
{
    protected DatabaseHandler databaseHandler;
    protected Type[] installedApps;

    protected Dictionary<string, ModelState> models = new Dictionary<string, ModelState>();

    protected Dictionary<Type, string> installedAppsNamespacesDict
    {
        get
        {
            var dict = new Dictionary<Type, string>();
            foreach (var app in installedApps)
            {
                dict[app] = app.Namespace!;
            }
            return dict;
        }
    }

    public List<(string ApplicationName, BaseMigration Migration)> MigrationsWithApps
    {
        get
        {
            var migrationInstances = CreateAllMigrationInstancesWithAppInstances();
            return migrationInstances;
        }
    }



    public MigrationRunner(DatabaseHandler databaseHandler, Type[] installedApps)
    {
        this.databaseHandler = databaseHandler;
        this.installedApps = installedApps;
    }

    public void RunMigrations()
    {
        databaseHandler.Connect();
        databaseHandler.CreateMigrationTable();
        var pendingMigrationWithApps = SortMigrationsWithGraph(MigrationsWithApps, databaseHandler.GetAppliedMigrations());
        var allMigrationsWithApps = SortMigrationsWithGraph(MigrationsWithApps, databaseHandler.GetAppliedMigrations(), onlyPending: false);
        foreach (var migrationWithApp in pendingMigrationWithApps)
        {
            var databaseState = new DatabaseState(databaseHandler, allMigrationsWithApps);
            var runner = new SingleMigrationRunner(migrationWithApp.Migration, databaseHandler, migrationWithApp.ApplicationName,databaseState);
            runner.RunMigration();
        }
        this.databaseHandler.Disconnect();
    }


    public List<(Type AppicationType, Type MigrationType)> GetAllMigrationTypesWithAppTypes()
    {
        var migrationTypesWithAppTypes = new List<(Type AppicationType, Type MigrationType)>();
        foreach (var namespacedApp in this.installedAppsNamespacesDict)
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null)!;
                    }
                });
            var nullableTypes = allTypes.Where(t => t!.IsClass && !t.IsAbstract && typeof(BaseMigration).IsAssignableFrom(t) && t.Namespace != null && t.Namespace.StartsWith(namespacedApp.Value));
            foreach (var type in nullableTypes)
            {
                migrationTypesWithAppTypes.Add((namespacedApp.Key, type!));
            }
        }
        return migrationTypesWithAppTypes;
    }

    public List<(string ApplicationName, BaseMigration Migration)> CreateAllMigrationInstancesWithAppInstances()
    {
        var migrationTypes = GetAllMigrationTypesWithAppTypes();
        var instances = new List<(string ApplicationName, BaseMigration Migration)>();

        foreach (var migrationTypeWithAppType in migrationTypes)
        {
            var migrationType = migrationTypeWithAppType.MigrationType;
            var appType = migrationTypeWithAppType.AppicationType;
            var migrationInstance = (BaseMigration)Activator.CreateInstance(migrationType)!;
            dynamic appInstance = Activator.CreateInstance(appType)!;
            instances.Add((appInstance.Name, migrationInstance));
        }
        return instances;
    }

    public List<(string ApplicationName, BaseMigration Migration)> SortMigrationsWithGraph(
        List<(string ApplicationName, BaseMigration Migration)> migrationsWithApps,
        List<(string ApplicationName, string MigrationName)> appliedMigrations,
        bool onlyPending = true)
    {
        var appliedKeys = new HashSet<string>(
            appliedMigrations.Select(m => $"{m.ApplicationName}|{m.MigrationName}"));

        List<(string ApplicationName, BaseMigration Migration)> filtredMigrations = migrationsWithApps;
        if (onlyPending)
            filtredMigrations = migrationsWithApps
            .Where(x => !appliedKeys.Contains($"{x.ApplicationName}|{x.Migration.Name}"))
            .ToList();

        var migrationDict = filtredMigrations.ToDictionary(
            x => $"{x.ApplicationName}|{x.Migration.Name}",
            x => x.Migration);

        var graph = new AdjacencyGraph<string, Edge<string>>();

        foreach (var key in migrationDict.Keys)
        {
            graph.AddVertex(key);
        }

        foreach (var migrationTuple in filtredMigrations)
        {
            string migrationKey = $"{migrationTuple.ApplicationName}|{migrationTuple.Migration.Name}";
            foreach (var dependency in migrationTuple.Migration.Dependencies)
            {
                string dependencyKey = $"{dependency.ApplicationName}|{dependency.MigrationName}";
                if (migrationDict.ContainsKey(dependencyKey))
                {
                    graph.AddEdge(new Edge<string>(dependencyKey, migrationKey));
                }
            }
        }

        IEnumerable<string> sortedKeys;
        try
        {
            sortedKeys = graph.TopologicalSort();
        }
        catch (Exception ex)
        {
            throw new Exception("Cycle detected in migration dependencies.", ex);
        }

        return sortedKeys.Select(key => (key.Split('|')[0], migrationDict[key])).ToList();
    }

}