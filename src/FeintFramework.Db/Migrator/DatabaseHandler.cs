namespace FeintFramework.Db.Migrator;

public abstract class DatabaseHandler
{
    protected string connectionString { get; set; }
    public MigrationOperationHandler? MigrationOperationHandler { get; protected set; }
    public DatabaseHandler(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public abstract void Connect();
    public abstract void Disconnect();

    public abstract void BeginTransaction();
    public abstract void CommitTransaction();
    public abstract void RollbackTransaction();
    public abstract void CreateMigrationTable();
    public abstract List<(string ApplicationName, string MigrationName)> GetAppliedMigrations();
    public abstract void ApplyMigration(string applicationName, string migrationName);
}