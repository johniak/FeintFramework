using System.Text.RegularExpressions;
using FeintFramework.Core.Config;
using FeintFramework.Core.Http;
using FeintFramework.Core.Http.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using QuikGraph;

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
            throw new Http404();
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

    public static string? Reverse(string urlName, Dictionary<string, object>? parameters = null)
    {
        var urlPatterns = Configurator.Settings.RootUrlPatterns.Urls;
        var reversedUrls = new Dictionary<string, string>();
        BuildReversedUrls(urlPatterns, reversedUrls, new List<string>(), "");
        if (reversedUrls.ContainsKey(urlName))
        {
            var regex = reversedUrls[urlName];
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

    public static List<(string Pattern, string? Name)> BuildFullUrlPatternList(List<UrlPattern> urlPatterns, List<(string Pattern, string? Name)>? fullPatternList = null, List<string>? baseNames = null, string baseUrl = "")
    {
        if (fullPatternList == null)
        {
            fullPatternList = new List<(string Pattern, string? Name)>();
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
                fullPatternList.Add((url, null));
                continue;
            }
            var finalNameList = new List<string>();
            finalNameList.AddRange(names);
            finalNameList.Add(urlPattern.Name);
            fullPatternList.Add((url, String.Join(":", finalNameList)));
        }
        return fullPatternList;
    }
}