using FeintFramework.Db.Migrator.Fields;
namespace FeintFramework.Db.Migrator.Sql;



public abstract class SqlField<T> : ISqlField where T : BaseField
{

    public abstract string GetSqlType(T field);

    public virtual string? GetSqlDefaultValue(T field)
    {
        return field.DefaultValue?.ToString();
    }

    public abstract List<string> GetSqlAttributes(T field);

    public string GetSqlType(BaseField field)
    {
        return GetSqlType((T)field);
    }

    public string? GetSqlDefaultValue(BaseField field)
    {
        return GetSqlDefaultValue((T)field);
    }

    public List<string> GetSqlAttributes(BaseField field)
    {
        return GetSqlAttributes((T)field);
    }
}