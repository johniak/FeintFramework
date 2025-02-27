
namespace FeintFramework.Db.Migrator;

public delegate string SqlGenerator(MigrationOperation operation);

public abstract class SqlGenerator<T> where T : MigrationOperation
{
    public abstract string GenerateSql(T operation);
}
public abstract class MigrationOperationHandler
{
    public string ConnectionString { get; protected set; }
    public abstract SqlGenerator[] OperationHandlers { get; }
    public MigrationOperationHandler(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    public abstract void HandleOperation(MigrationOperation operation);

}