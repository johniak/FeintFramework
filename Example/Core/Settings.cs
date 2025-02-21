using FeinFramework.Core.Settings;

namespace Example.Core{
    public class Settings : BaseSettings{
        public override List<Type> InstalledApps => new List<Type>{
            typeof(ExampleApp)
        };
    }
}