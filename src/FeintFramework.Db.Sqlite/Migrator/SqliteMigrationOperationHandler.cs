using FeintFramework.Db.Migrator;

namespace FeintFramework.Db.Sqlite.Migrator;

class SqliteMigrationOperationHandler : MigrationOperationHandler
{
    public override ISqlGenerator[] OperationHandlers
    {
        get
        {
            return [
                new CreateModelGenerator()
            ];
        }
    }


    protected SqliteDatabaseHandler databaseHandler;

    public SqliteMigrationOperationHandler(SqliteDatabaseHandler databaseHandler)
    {
        this.databaseHandler = databaseHandler;
    }

    public override void HandleForwrdOperation(MigrationOperation operation, string appName)
    {
        var operationType = operation.GetType();
        if (!OperationHandlersDict.ContainsKey(operationType))
        {
            throw new Exception($"Operation {operationType.Name} not supported");
        }
        var handler = OperationHandlersDict[operationType];
        var sql = handler.GenerateForwardSql(operation, appName);
        databaseHandler.ExecuteNonQuery(sql);
    }

    public override void HandleReverseOperation(MigrationOperation operation, string appName)
    {
        var operationType = operation.GetType();
        if (!OperationHandlersDict.ContainsKey(operationType))
        {
            throw new Exception($"Operation {operationType.Name} not supported");
        }
        var handler = OperationHandlersDict[operationType];
        var sql = handler.GenerateReverseSql(operation, appName);
        databaseHandler.ExecuteNonQuery(sql);
    }
}