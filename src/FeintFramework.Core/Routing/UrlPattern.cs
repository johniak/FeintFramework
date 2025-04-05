
using System.Text.RegularExpressions;
using FeintFramework.Core.Http;

namespace FeintFramework.Core.Routing;
public class UrlPattern
{
    private string? regexPattern;
    public string RegexPattern
    {
        get
        {
            if (this.Patterns == null)
            {
                return $"{this.regexPattern}";
            }
            return $"{this.regexPattern}";
        }
        protected set
        {
            regexPattern = value;
        }
    }
    public string? Name { get; protected set; }
    public RequestHandler? Handler;
    public List<UrlPattern>? Patterns { get; protected set; }
    public UrlPattern(string regexPattern, RequestHandler handler, string? name = null)
    {
        this.RegexPattern = regexPattern;
        this.Name = name;
        this.Handler = handler;
    }
    public UrlPattern(string regexPattern, List<UrlPattern> patterns, string? name = null)
    {
        this.RegexPattern = regexPattern;
        this.Name = name;
        this.Patterns = patterns;
    }


    public bool Match(string url)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(url, this.RegexPattern);
    }



    public Dictionary<string, string> GetStringParams(string url)
    {
        var regex = new Regex(this.RegexPattern, RegexOptions.Compiled);
        var match = regex.Match(url);
        if (!match.Success)
        {
            return new Dictionary<string, string>();
        }

        return regex.GetGroupNames()
                    .Where(name => name != "0" && match.Groups[name].Success)
                    .ToDictionary(
                        name => name,
                        name => match.Groups[name].Value
                    );
    }

    public override string ToString()
    {
        return this.Name ?? this.RegexPattern;
    }

}