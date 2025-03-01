
using System.Text.RegularExpressions;

namespace FeintFramework.Db.Migrator;
public interface ISqlGenerator
{
    string GenerateForwardSql(MigrationOperation operation, string appName);
    string GenerateReverseSql(MigrationOperation operation, string appName);
}

public abstract class SqlGenerator<T> : ISqlGenerator where T : MigrationOperation
{
    public abstract string GenerateForwardSql(T operation, string appName);

    public string GenerateForwardSql(MigrationOperation operation, string appName)
    {
        return GenerateForwardSql((T)operation, appName);
    }

    public abstract string GenerateReverseSql(T operation, string appName);

    public string GenerateReverseSql(MigrationOperation operation, string appName)
    {
        return GenerateReverseSql((T)operation, appName);
    }

    public static string ToUnderscoreCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        string result = Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2");
        return result.ToLowerInvariant();
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
    public abstract void HandleForwrdOperation(MigrationOperation operation, string appName);
    public abstract void HandleReverseOperation(MigrationOperation operation, string appName);

}