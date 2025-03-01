
namespace FeintFramework.Db.Migrator;
public class SingleMigrationRunner
{
    protected BaseMigration migration;
    protected DatabaseHandler databaseHandler;
    protected string appName;
    public SingleMigrationRunner(BaseMigration migration, DatabaseHandler databaseHandler, string appName)
    {
        this.migration = migration;
        this.databaseHandler = databaseHandler;
        this.appName = appName;
    }
    public void RunMigration()
    {
        if (migration.Atomic)
            databaseHandler.BeginTransaction();
        try
        {
            foreach (var operation in migration.Operations)
            {
                databaseHandler.MigrationOperationHandler?.HandleForwrdOperation(operation, appName);
            }
            if (migration.Atomic)
                databaseHandler?.CommitTransaction();
        }
        catch (Exception)
        {
            if (migration.Atomic)
                databaseHandler.RollbackTransaction();
            throw;
        }
    }
}