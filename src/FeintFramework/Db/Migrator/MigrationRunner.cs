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

    protected MigrationHelper migrationHelper;



    public MigrationRunner(DatabaseHandler databaseHandler, Type[] installedApps)
    {
        this.databaseHandler = databaseHandler;
        this.installedApps = installedApps;
        this.migrationHelper = new MigrationHelper(installedApps);
    }

    public void RunMigrations()
    {
        databaseHandler.Connect();
        databaseHandler.CreateMigrationTable();
        var migrationsWithApps = migrationHelper.CreateAllMigrationInstancesWithAppInstances();
        var pendingMigrationWithApps = migrationHelper.SortMigrationsWithGraph(migrationsWithApps, databaseHandler.GetAppliedMigrations());
        var allMigrationsWithApps = migrationHelper.SortMigrationsWithGraph(migrationsWithApps, []);
        foreach (var migrationWithApp in pendingMigrationWithApps)
        {
            var databaseState = new DatabaseState(databaseHandler, allMigrationsWithApps);
            var runner = new SingleMigrationRunner(migrationWithApp.Migration, databaseHandler, migrationWithApp.ApplicationName,databaseState);
            runner.RunMigration();
        }
        this.databaseHandler.Disconnect();
    }




}