

using FeintFramework.Core.Routing;
namespace Example{
class MainUrlPatterns : UrlPatterns
{
    public override List<UrlPattern> Urls
    {
        get
        {
            return new List<UrlPattern>
            {
                new UrlPattern("Home", "/"),
            }
        }
    }
}
}
