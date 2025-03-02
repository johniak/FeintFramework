using System.Text;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator.Operations;
using FeintFramework.Db.Migrator.Sql;

namespace FeintFramework.Db.Sqlite.Migrator;

public class AlterFieldGenerator : SqlGenerator<AlterField>
{
    public override string GenerateForwardSql(AlterField operation, string appName, DatabaseState databaseState)
    {

        var modelState = databaseState.GetModelState(appName, operation.ModelName);
        var fields = new Dictionary<string, BaseField>(modelState.Fields);
        fields[operation.Name] = operation.Field;
        var sqls = new string[]{
            createNewTable(operation, appName, databaseState,fields),
            copyData(operation, appName, databaseState),
            dropOldTable(operation, appName),
            renameTable(operation, appName)};
        return string.Join("\n", sqls);
    }


    public override string GenerateReverseSql(AlterField operation, string appName, DatabaseState databaseState)
    {

        var modelState = databaseState.GetModelState(appName, operation.ModelName);
        var fields = new Dictionary<string, BaseField>(modelState.Fields);
        var sqls = new string[]{
            createNewTable(operation, appName, databaseState,fields),
            copyData(operation, appName, databaseState),
            dropOldTable(operation, appName),
            renameTable(operation, appName)};
        return string.Join("\n", sqls);
    }

    protected string createNewTable(AlterField operation, string appName, DatabaseState databaseState, Dictionary<string, BaseField> newFields)
    {
        var createModelOperation = new CreateModel($"{operation.ModelName}__temp")
        {
            Fields = newFields.Select(kvp => (kvp.Key, kvp.Value)).ToArray()
        };
        return new CreateModelGenerator().GenerateForwardSql(createModelOperation, appName, databaseState);
    }

    protected string copyData(AlterField operation, string appName, DatabaseState databaseState)
    {
        var copyDataSql = $"INSERT INTO {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()}__temp SELECT * FROM {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()};";
        return copyDataSql;
    }

    protected string dropOldTable(AlterField operation, string appName)
    {
        return $"DROP TABLE {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()};";
    }

    protected string renameTable(AlterField operation, string appName)
    {
        return $"ALTER TABLE {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()}__temp RENAME TO {appName.ToUnderscoreCase()}_{operation.ModelName.ToUnderscoreCase()};";
    }

}