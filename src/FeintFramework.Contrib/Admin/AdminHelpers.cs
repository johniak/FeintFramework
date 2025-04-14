using System.Reflection;
using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using static FeintFramework.Core.Config.Configurator;

namespace FeintFramework.Contrib.Admin;

public record AppRecord(string ApplicationName, Type AppicationType, List<ModelAdmin> ModelAdmins);

public static class AdminHelpers
{
    private static List<AppRecord>? appRecords = null;
    public static List<AppRecord> AppRecords
    {
        get
        {
            Console.WriteLine("Getting app records");
            if (appRecords == null)
            {
                var migrationHelper = new MigrationHelper(Settings.InstalledApps);
                var modelAppsWithAppsTypes = migrationHelper.GetAllModelTypesWithAppTypes().Select(t => t.AppicationType).Distinct().ToList();
                var modelAdmins = GetModelAdmins();
                appRecords = new List<AppRecord>();
                Dictionary<Type, AppRecord> appRecordDict = new Dictionary<Type, AppRecord>();

                foreach (ModelAdmin modelAdmin in modelAdmins)
                {
                    var appType = modelAdmin.AppType;
                    if (!appRecordDict.ContainsKey(appType))
                    {
                        var appRecord = new AppRecord(modelAdmin.AppName, appType, new List<ModelAdmin>());
                        appRecordDict[appType] = appRecord;
                    }
                    appRecordDict[appType].ModelAdmins.Add(modelAdmin);
                }
                foreach (var keyValuePair in appRecordDict)
                {
                    var appRecord = keyValuePair.Value;
                    if (modelAppsWithAppsTypes.Contains(appRecord.AppicationType))
                    {
                        appRecords.Add(appRecord);
                    }
                }

            }
            return appRecords!;
        }
    }

    public static (string FieldName, BaseField Field)[] GetModelFields(Type modelType)
    {
        var migrationHelper = new MigrationHelper(Settings.InstalledApps);
        return migrationHelper.GetModelFields(modelType);
    }

    public static ModelAdmin[] GetModelAdmins()
    {
        var modelAdminTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null)!;
                }
            })
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ModelAdmin).IsAssignableFrom(t));
        var modelAdmins = new List<ModelAdmin>();
        foreach (var modelAdminType in modelAdminTypes)
        {
            var modelAdmin = (ModelAdmin)Activator.CreateInstance(modelAdminType)!;
            modelAdmins.Add(modelAdmin);
        }
        return modelAdmins.ToArray();
    }
}