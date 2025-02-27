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
}