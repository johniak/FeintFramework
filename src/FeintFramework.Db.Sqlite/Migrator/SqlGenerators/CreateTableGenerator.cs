using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class CreateModelGenerator : ISqlGenerator<CreateModel>
{
    public string GenerateForwardSql(CreateModel operation)
    {
        var builder = new StringBuilder();
        builder.Append($"CREATE TABLE {operation.TableName} (");
        var columns = new List<string>();
        foreach (var field in operation.Fields)
        {
            var sqlField = SqlFieldRegistry.GetField(field);
            columns.Add($"{field.Name} {sqlField.GetSqlType(field)} {string.Join(" ", sqlField.GetSqlAttributes(field))}");
        }
        builder.Append(string.Join(",\n", columns));
        builder.Append(");");
        return builder.ToString();
    }

    public string GenerateReverseSql(CreateModel operation)
    {
        return $"DROP TABLE {operation.TableName};";
    }
}