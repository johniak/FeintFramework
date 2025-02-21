using System.Text.RegularExpressions;

namespace FeinFramework.Core.Routing
{
    public class Path : UrlPattern
    {
        public string PathPattern { get; }
        public static readonly Dictionary<string, Func<string, object>> Parsers = new Dictionary<string, Func<string, object>>(StringComparer.OrdinalIgnoreCase)
        {
            { "int", s =>
                {
                    if (int.TryParse(s, out int result))
                        return result;
                    throw new FormatException($"Cannot parse '{s}' as int.");
                }
            },
            { "slug", s => s },
            { "str", s => s }
        };
        public Path(string pathPattern) : base(ConvertPatternToRegex(pathPattern))
        {

            this.PathPattern = pathPattern;
        }
        static string ConvertPatternToRegex(string pattern)
        {
            pattern = pattern.TrimEnd('/');
            pattern = Regex.Replace(pattern, @"<int:(\w+)>", @"(?<$1>\d+)");
            pattern = Regex.Replace(pattern, @"<str:(\w+)>", @"(?<$1>[^/]+)");
            pattern = Regex.Replace(pattern, @"<slug:(\w+)>", @"(?<$1>[^/]+)");
            return $"^{pattern}$";
        }

        public Dictionary<string, object> GetParams(string url)
        {
            var stringParams = this.GetStringParams(url);
            var paramTypes = this.ParameterTypes;
            foreach (var param in paramTypes)
            {
                var type = paramTypes[param.Key];
                var parser = Parsers[type.Name.ToLowerInvariant()];

            }
            return result;
        }

        public Dictionary<string, ParameterType<object>> ParameterTypes
        {
            get
            {
                var paramTypes = new Dictionary<string, Type>();
                // This regex finds tokens like <int:year>, <slug:slug>, etc.
                var paramRegex = new Regex(@"<(?<type>\w+):(?<name>\w+)>");
                var matches = paramRegex.Matches(this.PathPattern);
                foreach (Match match in matches)
                {
                    string typeString = match.Groups["type"].Value.ToLowerInvariant();
                    string nameString = match.Groups["type"].Value.ToString();
                    switch (typeString)
                    {
                        case "int":
                            paramTypes[nameString] = (typeof(int));
                            break;
                        case "slug":
                        case "str":
                            paramTypes[nameString] = (typeof(string));
                            break;
                        default:
                            // Extend with additional type mappings as needed.
                            paramTypes[nameString] = (typeof(string));
                            break;
                    }
                }
                return paramTypes;
            }
        }
    }
}

public abstract class ParameterType<T>
{
    public string Name { get; protected set; }

    public abstract T ToInternalValue(string value);

    public Type GetInternalType()
    {
        return typeof(T);
    }
}

public class StringParameter : ParameterType<string>
{
    public StringParameter()
    {
        this.Name = "str";
    }

    public override string ToInternalValue(string value)
    {
        return value;
    }
}

public class SlugParameter : StringParameter
{
    public SlugParameter()
    {
        this.Name = "slug";
    }
}

public class IntParameter : ParameterType<int>
{
    public IntParameter()
    {
        this.Name = "int";
    }

    public override int ToInternalValue(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        throw new FormatException($"Cannot parse '{value}' as int.");
    }
}