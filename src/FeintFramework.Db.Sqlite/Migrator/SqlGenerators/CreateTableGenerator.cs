using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class CreateModelGenerator : SqlGenerator<CreateModel>
{
    public override string GenerateForwardSql(CreateModel operation, string appName)
    {
        var builder = new StringBuilder();
        builder.Append($"CREATE TABLE {appName.ToUnderscoreCase()}_{operation.Name.ToLowerInvariant()} (\n");
        var columns = new List<string>();
        foreach (var field in operation.Fields)
        {
            var sqlField = SqlFieldRegistry.GetField(field);
            columns.Add($"{field.Name} {sqlField.GetSqlType(field)} {string.Join(" ", sqlField.GetSqlAttributes(field))}");
        }
        builder.Append(string.Join(",\n", columns));
        builder.Append("\n);");
        return builder.ToString();
    }


    public override string GenerateReverseSql(CreateModel operation, string appName)
    {
        return $"DROP TABLE {appName.ToUnderscoreCase()}_{operation.Name.ToLowerInvariant()};";
    }

}