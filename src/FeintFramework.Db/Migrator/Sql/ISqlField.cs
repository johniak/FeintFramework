using FeintFramework.Db.Migrator.Fields;
namespace FeintFramework.Db.Migrator.Sql;

public interface ISqlField
{

    public abstract string GetSqlType(BaseField field);

    public abstract string? GetSqlDefaultValue(BaseField field);

    public abstract List<string> GetSqlAttributes(BaseField field);

}