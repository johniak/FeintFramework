using System.Reflection;
using Microsoft.VisualBasic;

namespace FeintFramework.Db.Migrator;

class MigrationRunner
{
    public List<BaseMigration> Migrations
    {
        get
        {
            var migrationInstances = CreateAllMigrationInstances();
            return migrationInstances;
        }
    }
    protected DatabaseHandler databaseHandler;
    public MigrationRunner(DatabaseHandler databaseHandler)
    {
        this.databaseHandler = databaseHandler;
    }

    public void RunMigrations()
    {
        foreach (var migration in Migrations)
        {
            var runner = new SingleMigrationRunner(migration, databaseHandler);
            runner.RunMigration();
        }
    }


    public static List<Type> GetAllMigrationTypes()
    {
        var migrationTypes = new List<Type>();
        var nullableTypes = AppDomain.CurrentDomain.GetAssemblies()
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
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseMigration).IsAssignableFrom(t));
        foreach (var type in nullableTypes)
        {
            migrationTypes.Add(type!);
        }
        return migrationTypes;
    }

    public static List<BaseMigration> CreateAllMigrationInstances()
    {
        var migrationTypes = GetAllMigrationTypes();
        var instances = new List<BaseMigration>();

        foreach (var type in migrationTypes)
        {
            try
            {
                // Ensure the type has a parameterless constructor.
                if (type.GetConstructor(Type.EmptyTypes) != null)
                {
                    var instance = (BaseMigration)Activator.CreateInstance(type)!;
                    instances.Add(instance);
                }
                else
                {
                    Console.WriteLine($"Type {type.FullName} does not have a parameterless constructor.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create instance of {type.FullName}: {ex.Message}");
            }
        }

        return instances;
    }

}