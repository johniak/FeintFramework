using System.Text.RegularExpressions;
using FeintFramework.Core.Http;

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
        var url = matchPath(path, urlPatterns.Urls);
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

    protected UrlPattern? matchPath(string path, List<UrlPattern> urls)
    {
        foreach (var url in urls)
        {
            if (!url.Match(path))
                continue;
            if (url.Handler != null)
                return url;
            var newPath = Regex.Replace(path, url.RegexPattern, "");
            return matchPath(newPath, url.Patterns!);
        }
        return null;
    }
}