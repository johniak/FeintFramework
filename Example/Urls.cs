

using FeintFramework.Core.Http;
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
                new UrlPattern("/example", (request) =>
                {
                    return new FeintHttpResponse
                    {
                        StatusCode = 200,
                        Content = "OK"
                    };
                }),
            };
        }
    }
}
}
