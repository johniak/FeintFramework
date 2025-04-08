using FeintFramework.Core.Config.Settings;
using FeintFramework.Core.Routing;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Sqlite.Migrator;
using FeintFramework.Contrib.Sessions;
using FeintFramework.Contrib.Auth;

namespace Example.Core
{
    public class Settings : BaseSettings
    {
        public override Type[] InstalledApps => [
            typeof(Blog.BlogApp),
            typeof(SessionsApp),
            typeof(AuthApp)
        ];


        public override RootUrlPatterns RootUrlPatterns => new MainUrlPatterns();

        public override List<Type> Middlewares => new List<Type>
        {
            typeof(SessionMiddleware),
            typeof(AuthMiddleware),
        };

        public override DatabaseHandler DatabaseHandler => new SqliteDatabaseHandler(DatabaseConnectionString);

        protected override void ConfigureAdditionalSettings()
        {
            
        }
    }
}