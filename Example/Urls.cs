

using FeintFramework.Contrib.Admin;
using FeintFramework.Http;
using FeintFramework.Routing;
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
                new FeintFramework.Routing.Path("/example2/<int:test>", new ExampleView().AsView, "example2"),
            
                new UrlPattern("",new AdminUrls().Urls)
            };
            }
        }
    }
}
