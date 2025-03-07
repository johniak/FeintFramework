
using System.Reflection;
using FeintFramework.Db.Migrator;
using QuikGraph;
using QuikGraph.Algorithms;

namespace FeintFramework.Db;
public class MigrationHelper
{
    protected Type[] installedApps;

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

    public MigrationHelper(Type[] installedApps)
    {
        this.installedApps = installedApps;
    }

    public List<(Type AppicationType, Type ModelType)> GetAllModelTypesWithAppTypes()
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
            var nullableTypes = allTypes.Where(t => t!.IsClass && !t.IsAbstract && typeof(Model).IsAssignableFrom(t) && t.Namespace != null && t.Namespace.StartsWith(namespacedApp.Value));
            foreach (var type in nullableTypes)
            {
                migrationTypesWithAppTypes.Add((namespacedApp.Key, type!));
            }
        }
        return migrationTypesWithAppTypes;
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
        List<(string ApplicationName, string MigrationName)> appliedMigrations)
    {
        var appliedKeys = new HashSet<string>(
            appliedMigrations.Select(m => $"{m.ApplicationName}|{m.MigrationName}"));

        List<(string ApplicationName, BaseMigration Migration)> filtredMigrations = migrationsWithApps;
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

    public int GetCurrentMigrationNumber(string applicationName)
    {
        var migriationsWithTypes = GetAllMigrationTypesWithAppTypes();
        var migrations = migriationsWithTypes.Where(m => m.AppicationType.Name == applicationName);
        return migrations.Count();
    }
    public Dictionary<string, int> GetCurrentMigrationNumbers()
    {
        var migriationsWithTypes = GetAllMigrationTypesWithAppTypes();
        var migrationNumbers = new Dictionary<string, int>();
        foreach (var app in installedApps)
        {
            var migrations = migriationsWithTypes.Where(m => m.AppicationType.Name == app.Name);
            migrationNumbers[app.Name] = migrations.Count();
        }
        return migrationNumbers;
    }
}