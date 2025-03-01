using FeintFramework.Core.Config.Settings;
using FeintFramework.Core.Routing;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Sqlite.Migrator;

namespace Example.Core
{
    public class Settings : BaseSettings
    {
        public override Type[] InstalledApps => [
            typeof(ExampleApp.ExampleApp)
        ];


        public override UrlPatterns RootUrlPatterns => new MainUrlPatterns();

        public override List<Type> Middlewares => new List<Type>
        {

        };

        public override DatabaseHandler DatabaseHandler => new SqliteDatabaseHandler(DatabaseConnectionString);
    }
}