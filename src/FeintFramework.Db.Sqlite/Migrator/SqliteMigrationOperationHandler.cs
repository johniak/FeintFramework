using FeintFramework.Db.Migrator;

namespace FeintFramework.Db.Sqlite.Migrator;

class SqliteMigrationOperationHandler : MigrationOperationHandler
{
    public override ISqlGenerator<MigrationOperation>[] OperationHandlers
    {
        get
        {
            return [
            (ISqlGenerator<MigrationOperation>)new CreateModelGenerator()
        ];

        }
    }


    protected SqliteDatabaseHandler databaseHandler;

    public SqliteMigrationOperationHandler(SqliteDatabaseHandler databaseHandler)
    {
        this.databaseHandler = databaseHandler;
    }

    public override void HandleForwrdOperation(MigrationOperation operation)
    {
        var operationType = operation.GetType();
        if (!OperationHandlersDict.ContainsKey(operationType))
        {
            throw new Exception($"Operation {operationType.Name} not supported");
        }
        var handler = OperationHandlersDict[operationType];
        var sql = handler.GenerateForwardSql(operation);
        databaseHandler.ExecuteNonQuery(sql);
    }

    public override void HandleReverseOperation(MigrationOperation operation)
    {
        var operationType = operation.GetType();
        if (!OperationHandlersDict.ContainsKey(operationType))
        {
            throw new Exception($"Operation {operationType.Name} not supported");
        }
        var handler = OperationHandlersDict[operationType];
        var sql = handler.GenerateReverseSql(operation);
        databaseHandler.ExecuteNonQuery(sql);
    }
}