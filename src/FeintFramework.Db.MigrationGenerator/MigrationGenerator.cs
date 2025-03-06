using System.Reflection;
using FeintFramework.Core.Apps;
using FeintFramework.Core.Config.Settings;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FeintFramework.Db.MigrationGenerator;
public class MigrationGenerator
{
    DatabaseState databaseState;
    MigrationHelper migrationHelper;
    public MigrationGenerator(BaseSettings settings)
    {
        migrationHelper = new MigrationHelper(settings.InstalledApps);
        databaseState = new DatabaseState(migrationHelper.CreateAllMigrationInstancesWithAppInstances());
    }

    public void GenerateMigration()
    {
        var installedModels = migrationHelper.GetAllModelTypesWithAppTypes();
        var modelsGroupedByApp = installedModels.GroupBy(m => m.AppicationType);
        foreach(var groupedByModel in modelsGroupedByApp)
        {
            var appType = groupedByModel.Key;
            var models = groupedByModel.Select(m => m.ModelType);
            var appNamespace = appType.Namespace!;
            var migrationClassName = $"_004_migration";
            var appInstance = (BaseApplication)Activator.CreateInstance(appType)!;
            var classGenerator = new ClassGenerator(appInstance, migrationClassName);
            // foreach(var model in )
            Console.WriteLine(classGenerator.ToString());
        }
    }
    

    protected (string FieldName, BaseField Field)[] getModelFields(Type modelType)
    {
        var fields = modelType.GetProperties();
        var fieldList = new List<(string FieldName, BaseField Field)>();
        foreach (var field in fields)
        {
            var baseField = field.GetCustomAttribute<BaseField>();
            if (baseField != null)
            {
                fieldList.Add((field.Name, baseField));
            }
        }
        return fieldList.ToArray();
    }

}
