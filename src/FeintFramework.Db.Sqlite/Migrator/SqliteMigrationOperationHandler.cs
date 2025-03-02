using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace FeintFramework.Db.Sqlite.Migrator;

class SqliteMigrationOperationHandler : MigrationOperationHandler
{
    public override ISqlGenerator[] OperationHandlers
    {
        get
        {
            return [
                new CreateModelGenerator(),
                new AddFieldGenerator(),
                new RemoveFieldGenerator(),
                new AlterFieldGenerator(),
                new RunSqlGenerator()
            ];
        }
    }


    protected SqliteDatabaseHandler databaseHandler;

    public SqliteMigrationOperationHandler(SqliteDatabaseHandler databaseHandler)
    {
        this.databaseHandler = databaseHandler;
    }

    public override void HandleForwardOperation(MigrationOperation operation, string appName, DatabaseState databaseState)
    {
        var operationType = operation.GetType();
        if (!OperationHandlersDict.ContainsKey(operationType))
        {
            throw new Exception($"Operation {operationType.Name} not supported");
        }
        var handler = OperationHandlersDict[operationType];
        var sql = handler.GenerateForwardSql(operation, appName, databaseState);
        databaseHandler.ExecuteNonQuery(sql);
    }

    public override void HandleReverseOperation(MigrationOperation operation, string appName, DatabaseState databaseState)
    {
        var operationType = operation.GetType();
        if (!OperationHandlersDict.ContainsKey(operationType))
        {
            throw new Exception($"Operation {operationType.Name} not supported");
        }
        var handler = OperationHandlersDict[operationType];
        var sql = handler.GenerateReverseSql(operation, appName, databaseState);
        databaseHandler.ExecuteNonQuery(sql);
    }
}