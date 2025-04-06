

using FeintFramework.Contrib.Admin;
using FeintFramework.Core.Http;
using FeintFramework.Core.Routing;
namespace Example
{
    class MainUrlPatterns : RootUrlPatterns
    {
        public override List<UrlPattern> Urls
        {
            get
            {
                return new List<UrlPattern>
            {
                new UrlPattern("/example", new ExampleView().AsView, "example"),
                new FeintFramework.Core.Routing.Path("/example2/<int:test>", new ExampleView().AsView, "example2"),
            
                new UrlPattern("",new AdminUrls().Urls)
            };
            }
        }
    }
}
