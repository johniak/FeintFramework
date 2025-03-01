using System.Reflection.Metadata;
using FeintFramework.Core.Routing;
using FeintFramework.Db.Migrator;

namespace FeintFramework.Core.Config.Settings
{
    public abstract class BaseSettings
    {
        public abstract Type[] InstalledApps { get; }
        public abstract UrlPatterns RootUrlPatterns { get; }
        public abstract List<Type> Middlewares { get; }
        public virtual String DatabaseConnectionString
        {
            get
            {
                return "Data Source=./db.sqlite";
            }
        }
        public virtual String DatabaseProvider
        {
            get
            {
                return "Microsoft.Data.Sqlite";
            }
        }
        public abstract DatabaseHandler DatabaseHandler { get; }
    }
}