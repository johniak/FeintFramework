using FeintFramework.Core.Config.Settings;
using FeintFramework.Core.Routing;

namespace Example.Core
{
    public class Settings : BaseSettings
    {
        public override List<Type> InstalledApps => new List<Type>{
            typeof(ExampleApp.ExampleApp)
        };

        public override UrlPatterns RootUrlPatterns => new MainUrlPatterns();
    }
}