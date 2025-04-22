using FeintFramework.Config.Settings;
using FeintFramework.Routing;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Sqlite.Migrator;
using FeintFramework.Contrib.Sessions;
using FeintFramework.Contrib.Auth;
using FeintFramework.Templating;
using FeintFramework.Templating.Tags;
using FeintFramework.Contrib.Admin.TemplateTags;

namespace Example.Core
{
    public class Settings : BaseSettings
    {
        public override bool Debug => true;
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

        public override void ConfigureAdditionalSettings()
        {
            TagRegistry.Register("admin_url", AdminUrlTag.ParseAdminUrlTag);
        }
        
    }
}