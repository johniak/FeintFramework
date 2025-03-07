
using System.Text.RegularExpressions;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator.Operations;
namespace FeintFramework.Db.Migrator;
public interface ISqlGenerator
{
    string GenerateForwardSql(MigrationOperation operation, string appName, DatabaseState databaseState);
    string GenerateReverseSql(MigrationOperation operation, string appName, DatabaseState databaseState);
}

public abstract class SqlGenerator<T> : ISqlGenerator where T : MigrationOperation
{
    public abstract string GenerateForwardSql(T operation, string appName, DatabaseState databaseState);

    public string GenerateForwardSql(MigrationOperation operation, string appName, DatabaseState databaseState)
    {
        return GenerateForwardSql((T)operation, appName, databaseState);
    }

    public abstract string GenerateReverseSql(T operation, string appName, DatabaseState databaseState);

    public string GenerateReverseSql(MigrationOperation operation, string appName, DatabaseState databaseState)
    {
        return GenerateReverseSql((T)operation, appName, databaseState);
    }
    public static string GetTableName(string appName, ModelOperation operation)
    {
        return $"{appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()}";
    }

    public static string GetColumnName(string fieldName, BaseField field)
    {
        var columnName = fieldName.ToUnderscoreCase();
        if (field is ForeignKey)
        {
            return $"{columnName}_id";
        }
        return columnName;
    }
}
public abstract class MigrationOperationHandler
{
    public abstract ISqlGenerator[] OperationHandlers { get; }

    protected Dictionary<Type, ISqlGenerator> operationHandlersDict = new Dictionary<Type, ISqlGenerator>();

    public Dictionary<Type, ISqlGenerator> OperationHandlersDict
    {
        get
        {
            if (operationHandlersDict.Count == 0)
            {
                foreach (var handler in OperationHandlers)
                {
                    operationHandlersDict.Add(GetGenericTypeFromSqlGenerator(handler), handler);
                }
            }
            return operationHandlersDict;
        }
    }
    public static Type GetGenericTypeFromSqlGenerator(object generator)
    {
        var generatorType = generator.GetType();
        Type? baseType = generatorType.BaseType;
        return baseType!.GetGenericArguments()[0];
    }
    public abstract void HandleForwardOperation(MigrationOperation operation, string appName, DatabaseState databaseState);
    public abstract void HandleReverseOperation(MigrationOperation operation, string appName, DatabaseState databaseState);

}