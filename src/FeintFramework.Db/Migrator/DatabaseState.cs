

using FeintFramework.Db.Migrator.Operations;

namespace FeintFramework.Db.Migrator;

public class DatabaseState
{
    protected Dictionary<string, ModelState> ModelStates { get; set; } = new Dictionary<string, ModelState>();

    public DatabaseState(DatabaseHandler databaseHandler, List<(String ApplicationName, BaseMigration Migration)> allMigrations)
    {
        var appliedMigrationsFromDb = databaseHandler.GetAppliedMigrations();
        var appliedKeys = new HashSet<string>(
             appliedMigrationsFromDb.Select(m => $"{m.ApplicationName}|{m.MigrationName}"));
        var appliedMigrations = allMigrations.Where(m => appliedKeys.Contains($"{m.ApplicationName}|{m.Migration.Name}"));

        foreach (var migration in appliedMigrations)
        {
            foreach (var operation in migration.Migration.Operations)
            {
                if (operation is not ModelOperation)
                    continue;
                var modelOperation = (ModelOperation)operation;
                var modelKey = $"{migration.ApplicationName}|{modelOperation.ModelName}";
                ModelStates[modelKey] = ModelStates.TryGetValue(modelKey, out var modelState) ? modelState : new ModelState();
                modelState = ModelStates[modelKey];
                modelState.applyOperation(modelOperation);
            }
        }
    }

    public ModelState GetModelState(string appName, string modelName)
    {
        var key = $"{appName}|{modelName}";
        return ModelStates[key];
    }
}