namespace FeintFramework.Db.Migrator;

public abstract class DatabaseHandler
{
    public string ConnectionString { get; protected set; }
    public MigrationOperationHandler MigrationOperationHandler { get; protected set; }
    public DatabaseHandler(string connectionString, MigrationOperationHandler migrationOperationHandler)
    {
        ConnectionString = connectionString;
        MigrationOperationHandler = migrationOperationHandler;
    }

    public abstract void Connect();
    public abstract void Disconnect();
}