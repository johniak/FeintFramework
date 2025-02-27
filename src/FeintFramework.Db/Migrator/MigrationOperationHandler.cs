
namespace FeintFramework.Db.Migrator;

public interface ISqlGenerator<in T> where T : MigrationOperation
{
    string GenerateSql(T operation);
}
public abstract class MigrationOperationHandler
{
    public abstract ISqlGenerator<MigrationOperation>[] OperationHandlers { get; }

    protected Dictionary<Type, ISqlGenerator<MigrationOperation>> operationHandlersDict = new Dictionary<Type, ISqlGenerator<MigrationOperation>>();

    public Dictionary<Type, ISqlGenerator<MigrationOperation>> OperationHandlersDict
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
        var sqlGeneratorInterface = generatorType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISqlGenerator<>));
        return sqlGeneratorInterface!.GetGenericArguments()[0];
    }
    public abstract void HandleOperation(MigrationOperation operation);

}