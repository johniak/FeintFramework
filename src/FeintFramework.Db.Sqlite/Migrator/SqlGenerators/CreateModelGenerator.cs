using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class CreateModelGenerator : SqlGenerator<CreateModel>
{
    public override string GenerateForwardSql(CreateModel operation, string appName, DatabaseState databaseState)
    {
        var builder = new StringBuilder();
        builder.Append($"CREATE TABLE {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()} (\n");
        var columns = new List<string>();
        foreach (var field in operation.Fields)
        {
            var sqlField = SqlFieldRegistry.GetField(field.Field);
            columns.Add($"{GetColumnName(field.Name, field.Field)} {sqlField.GetSqlType(field.Field)} {string.Join(" ", sqlField.GetSqlAttributes(field.Field))}");
        }
        builder.Append(string.Join(",\n", columns));
        builder.Append("\n);");
        return builder.ToString();
    }


    public override string GenerateReverseSql(CreateModel operation, string appName, DatabaseState databaseState)
    {
        return $"DROP TABLE {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()};";
    }

}