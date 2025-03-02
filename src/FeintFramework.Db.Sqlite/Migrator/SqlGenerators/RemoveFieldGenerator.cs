using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class RemoveFieldGenerator : SqlGenerator<RemoveField>
{
    public override string GenerateForwardSql(RemoveField operation, string appName, DatabaseState databaseState)
    {
        return $"ALTER TABLE {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()} DROP COLUMN {operation.Name};";
    }


    public override string GenerateReverseSql(RemoveField operation, string appName, DatabaseState databaseState)
    {
        throw new NotImplementedException();
    }

}