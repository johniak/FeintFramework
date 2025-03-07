using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class AddFieldGenerator : SqlGenerator<AddField>
{
    public override string GenerateForwardSql(AddField operation, string appName, DatabaseState databaseState)
    {
        var builder = new StringBuilder();
        var field = operation.Field;
        var sqlField = SqlFieldRegistry.GetField(field);
        builder.Append($"ALTER TABLE {GetTableName(appName, operation)} ");
        builder.Append($"ADD COLUMN {GetColumnName(operation.Name, field)} ");
        builder.Append($"{sqlField.GetSqlType(field)} {string.Join(" ", sqlField.GetSqlAttributes(field))};");
        return builder.ToString();
    }


    public override string GenerateReverseSql(AddField operation, string appName, DatabaseState databaseState)
    {
        return $"ALTER TABLE {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()} DROP COLUMN {operation.Name};";
    }

}