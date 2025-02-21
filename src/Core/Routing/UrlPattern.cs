
using System.Text.RegularExpressions;

namespace FeinFramework.Core.Routing
{
    public class UrlPattern
    {
        public string RegexPattern { get; protected set; }
        public string? Name { get; protected set; }
        public UrlPattern(string regexPattern, string? name = null)
        {
            this.RegexPattern = regexPattern;
            this.Name = name;
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
}