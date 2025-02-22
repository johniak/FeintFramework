using System.Reflection;
using System.Text.RegularExpressions;
using FeintFramework.Core.Http;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Core.Routing;
public class Path : UrlPattern
{
    protected Dictionary<string, IParameterType>? parameterTypes;
    public string PathPattern { get; }
    public Path(string pathPattern, Func<FeintHttpRequest, FeintHttpResponse> handler, string? name = null) : base(ConvertPatternToRegex(pathPattern), handler, name)
    {
        this.PathPattern = pathPattern;
    }
    public Path(string pathPattern, List<UrlPattern> patterns, string? name = null) : base(ConvertPatternToRegex(pathPattern), patterns, name)
    {
        this.PathPattern = pathPattern;
    }
    static string ConvertPatternToRegex(string pattern)
    {
        pattern = pattern.TrimEnd('/');
        pattern = Regex.Replace(pattern, @"<int:(\w+)>", @"(?<$1>\d+)");
        pattern = Regex.Replace(pattern, @"<str:(\w+)>", @"(?<$1>[^/]+)");
        pattern = Regex.Replace(pattern, @"<slug:(\w+)>", @"(?<$1>[^/]+)");
        return pattern;
    }

    public Dictionary<string, object> GetParams(string url)
    {
        var stringParams = this.GetStringParams(url);
        var paramTypes = this.ParameterTypes;
        Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
        foreach (var param in stringParams)
        {
            result[param.Key] = paramTypes[param.Key].ToInternalValue(param.Value);
        }
        return result;
    }
    public Dictionary<string, IParameterType> ParameterTypes
    {

        get
        {
            if (parameterTypes != null)
            {
                return parameterTypes;
            }
            var parameterTypeInstancesDict = (Dictionary<string, IParameterType>)Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => typeof(IParameterType).IsAssignableFrom(t)
                                                && !t.IsAbstract)
                                    .Select(t => (IParameterType)Activator.CreateInstance(t)!).ToDictionary(
                p => p.TypeName,
                p => p
            );
            var paramTypes = new Dictionary<string, IParameterType>();
            var paramRegex = new Regex(@"<(?<type>\w+):(?<name>\w+)>");
            var matches = paramRegex.Matches(this.PathPattern);
            foreach (Match match in matches)
            {
                string typeString = match.Groups["type"].Value.ToLowerInvariant();
                string nameString = match.Groups["type"].Value.ToString();
                paramTypes[nameString] = parameterTypeInstancesDict[typeString];
            }
            parameterTypes = paramTypes;
            return parameterTypes;
        }
    }
}

