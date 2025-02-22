using FeintFramework.Core.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace FeintFramework.Core.Routing;
public class Router
{
    private readonly UrlPatterns urlPatterns;

    public Router(UrlPatterns urlPatterns)
    {
        this.urlPatterns = urlPatterns;
    }

    public FeintHttpResponse HandleRequest(FeintHttpRequest request)
    {
        var path = request.Path;
        var url = matchPath(path);
        if (url == null)
        {
            return new FeintHttpResponse
            {
                StatusCode = 404,
                Content = "Not Found"
            };
        }
        var handler = url.Handler;
        return handler!(request);
    }

    // TODO: nested patterns
    protected UrlPattern? matchPath(string path)
    {
        foreach (var url in urlPatterns.Urls)
        {
            if (url.Match(path))
            {
                return url;
            }
        }
        return null;
    }
}