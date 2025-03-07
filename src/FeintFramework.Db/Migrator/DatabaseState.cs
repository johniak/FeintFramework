

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
        Initialize(allMigrations, appliedMigrations);
    }

    public DatabaseState(List<(String ApplicationName, BaseMigration Migration)> allMigrations)
    {
        Initialize(allMigrations, []);
    }

    public void Initialize(List<(String ApplicationName, BaseMigration Migration)> allMigrations, IEnumerable<(string ApplicationName, BaseMigration Migration)> appliedMigrations)
    {
        IEnumerable<(string ApplicationName, BaseMigration Migration)> migrations = allMigrations;
        if (appliedMigrations.Any())
        {
            migrations = appliedMigrations;
        }
        foreach (var migration in migrations)
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

    public ModelState? GetModelState(string appName, string modelName)
    {
        var key = $"{appName}|{modelName}";
        if (ModelStates.ContainsKey(key))
        {
            return ModelStates[key];
        }
        return null;
    }

    public string[] GetExistingModels()
    {
        return ModelStates.Keys.ToArray();
    }
}