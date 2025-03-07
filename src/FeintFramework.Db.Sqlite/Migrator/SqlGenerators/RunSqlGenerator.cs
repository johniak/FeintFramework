using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class RunSqlGenerator : SqlGenerator<RunSql>
{
    public override string GenerateForwardSql(RunSql operation, string appName, DatabaseState databaseState)
    {
        return operation.Sql;
    }


    public override string GenerateReverseSql(RunSql operation, string appName, DatabaseState databaseState)
    {
        return operation.ReverseSql;
    }

}