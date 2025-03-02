
namespace FeintFramework.Db.Migrator.Operations;
public class RunSql : MigrationOperation
{
    public string Sql { get; set; }
    public string ReverseSql { get; set; }
    public RunSql(string sql, string reverseSql)
    {
        this.Sql = sql;
        this.ReverseSql = reverseSql;
    }
}