
namespace FeintFramework.Db.Migrator;
public class SingleMigrationRunner
{
    protected BaseMigration migration;
    protected DatabaseHandler databaseHandler;
    protected string appName;
    protected DatabaseState databaseState;
    public SingleMigrationRunner(BaseMigration migration, DatabaseHandler databaseHandler, string appName, DatabaseState databaseState)
    {
        this.migration = migration;
        this.databaseHandler = databaseHandler;
        this.appName = appName;
        this.databaseState = databaseState;
    }
    public void RunMigration()
    {
        if (migration.Atomic)
            databaseHandler.BeginTransaction();
        try
        {
            foreach (var operation in migration.Operations)
            {
                databaseHandler.MigrationOperationHandler?.HandleForwardOperation(operation, appName, databaseState);
            }

            databaseHandler.ApplyMigration(this.appName, migration.Name);
            if (migration.Atomic)
                databaseHandler?.CommitTransaction();
        }
        catch (Exception e)
        {
            if (migration.Atomic)
                databaseHandler.RollbackTransaction();
            throw;
        }
    }
}