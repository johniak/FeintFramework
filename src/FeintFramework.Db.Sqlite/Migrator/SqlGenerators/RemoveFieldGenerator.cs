using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class RemoveFieldGenerator : SqlGenerator<RemoveField>
{
    public override string GenerateForwardSql(RemoveField operation, string appName, DatabaseState databaseState)
    {
        var modelState = databaseState.GetModelState(appName, operation.ModelName);
        var field = modelState!.Fields[operation.Name];
        return $"ALTER TABLE {GetTableName(appName, operation)} DROP COLUMN {GetColumnName(operation.Name, field)};";
    }


    public override string GenerateReverseSql(RemoveField operation, string appName, DatabaseState databaseState)
    {
        throw new NotImplementedException();
    }

}