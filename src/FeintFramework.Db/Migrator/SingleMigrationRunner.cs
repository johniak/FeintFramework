
namespace FeintFramework.Db.Migrator;
public class SingleMigrationRunner
{
    protected BaseMigration migration;
    protected DatabaseHandler databaseHandler;
    public SingleMigrationRunner(BaseMigration migration, DatabaseHandler databaseHandler)
    {
        this.migration = migration;
        this.databaseHandler = databaseHandler;
    }
    public void RunMigration()
    {
        if (migration.Atomic)
            databaseHandler.BeginTransaction();
        try
        {
            foreach (var operation in migration.Operations)
            {
                databaseHandler.MigrationOperationHandler?.HandleForwrdOperation(operation);
            }
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