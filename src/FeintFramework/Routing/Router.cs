using System.Text.RegularExpressions;
using FeintFramework.Config;
using FeintFramework.Http;
using FeintFramework.Http.Exceptions;
namespace FeintFramework.Routing;

public record MatchedUrl(string Pattern, string? Name, RequestHandler? Handler)
{
    public Dictionary<string, string> PathParams { get; set; } = new Dictionary<string, string>();
}
public class Router
{
    private readonly UrlPatterns urlPatterns;
    private List<(string Pattern, string? Name, RequestHandler handler)>? fullPatternList = null;

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
            throw new Http404();
        }
        var handler = url.Handler;
        foreach (var param in url.PathParams)
        {
            request.PathParams[param.Key] = param.Value;
        }
        return handler!(request);
    }

    protected MatchedUrl? matchPath(string path, List<UrlPattern> urls)
    {
        if (fullPatternList == null)
        {
            fullPatternList = BuildFullUrlPatternList(urls);
        }
        foreach (var url in fullPatternList)
        {
            var regex = new Regex(url.Pattern, RegexOptions.Compiled);
            var match = regex.Match(path);
            if (match.Success)
            {
                var pathParams = regex.GetGroupNames()
                    .Where(name => name != "0" && match.Groups[name].Success)
                    .ToDictionary(
                        name => name,
                        name => match.Groups[name].Value
                    );
                return new MatchedUrl(url.Pattern, url.Name, url.handler) { PathParams = pathParams };
            }
        }
        return null;
    }

    public static string? Reverse(string urlName, Dictionary<string, object>? parameters = null)
    {
        var urlPatterns = Configurator.Settings.RootUrlPatterns.Urls;
        var reversedUrls = new Dictionary<string, string>();
        BuildReversedUrls(urlPatterns, reversedUrls, new List<string>(), "");
        if (reversedUrls.ContainsKey(urlName))
        {
            var regex = reversedUrls[urlName];
            regex.Replace("^", "");
            regex = regex.Replace("$", "");
            if (parameters == null)
                return regex;
            return FillPattern(regex, parameters);
        }
        return null;
    }
    public static string FillPattern(string pattern, Dictionary<string, object> replacements)
    {
        string namedGroupPattern = @"\(\?<(?<name>\w+)>[^)]+\)";
        string filled = Regex.Replace(pattern, namedGroupPattern, match =>
        {
            string groupName = match.Groups["name"].Value;
            if (replacements.TryGetValue(groupName, out object? replacement))
            {
                return replacement!.ToString()!;
            }
            return match.Value;
        });

        return filled;
    }

    public static void BuildReversedUrls(List<UrlPattern> urlPatterns, Dictionary<string, string> reversedUrls, List<string> baseNames, string baseUrl)
    {
        var names = new List<string>();
        names.AddRange(baseNames);
        foreach (var urlPattern in urlPatterns)
        {
            var url = $"{baseUrl}{urlPattern.RegexPattern}";
            if (urlPattern.Patterns != null)
            {
                if (urlPattern.Name != null)
                {
                    names.Add(urlPattern.Name!);
                }
                BuildReversedUrls(urlPattern.Patterns!, reversedUrls, names, url);
                continue;
            }
            if (urlPattern.Name == null)
                continue;
            var finalNameList = new List<string>();
            finalNameList.AddRange(names);
            finalNameList.Add(urlPattern.Name);
            reversedUrls[String.Join(":", finalNameList)] = url;
        }
    }

    // TODO: Use records instead of tuples
    public static List<(string Pattern, string? Name, RequestHandler handler)> BuildFullUrlPatternList(List<UrlPattern> urlPatterns, List<(string Pattern, string? Name, RequestHandler handler)>? fullPatternList = null, List<string>? baseNames = null, string baseUrl = "")
    {
        if (fullPatternList == null)
        {
            fullPatternList = new List<(string Pattern, string? Name, RequestHandler handler)>();
        }
        if (baseNames == null)
        {
            baseNames = new List<string>();
        }
        var names = new List<string>();
        names.AddRange(baseNames);
        foreach (var urlPattern in urlPatterns)
        {
            var url = $"{baseUrl}{urlPattern.RegexPattern}";
            if (urlPattern.Patterns != null)
            {
                if (urlPattern.Name != null)
                {
                    names.Add(urlPattern.Name!);
                }
                BuildFullUrlPatternList(urlPattern.Patterns!, fullPatternList, names, url);
                continue;
            }
            if (urlPattern.Name == null)
            {
                fullPatternList.Add(($"^{url}$", null, urlPattern.Handler!));
                continue;
            }
            var finalNameList = new List<string>();
            finalNameList.AddRange(names);
            finalNameList.Add(urlPattern.Name);
            fullPatternList.Add(($"^{url}$", String.Join(":", finalNameList), urlPattern.Handler!));
        }
        return fullPatternList;
    }
}