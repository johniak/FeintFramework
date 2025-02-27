using FeintFramework.Db.Migrator.Fields;

namespace FeintFramework.Db.Migrator.Sql;

public static class SqlFieldRegistry
{
    private static Dictionary<Type, ISqlField> fields = new Dictionary<Type, ISqlField>();

    public static void RegisterField<T>(SqlField<T> field) where T : BaseField
    {
        fields.Add(typeof(T), field);
    }

    public static SqlField<T> GetField<T>() where T : BaseField
    {
        return
         (SqlField<T>)fields[typeof(T)];
    }
    public static ISqlField GetField(BaseField field)
    {
        return GetFieldRecursive(field.GetType());
    }
    private static ISqlField GetFieldRecursive(Type type)
    {
        if (fields.TryGetValue(type, out var sqlField))
        {
            return sqlField;
        }
        else if (type.BaseType != null)
        {
            return GetFieldRecursive(type.BaseType);
        }
        else
        {
            throw new KeyNotFoundException($"Field {type.Name} not registered");
        }
    }
}