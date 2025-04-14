

using System.Collections;
using System.Text;
using FeintFramework.Core.Templating.Node;
using static FeintFramework.Core.Shortcuts;
namespace FeintFramework.Core.Templating.Tags;

public class UrlNode : BaseNode{
    public string ViewName { get; }
    public List<string> PositionalArgs { get; }
    public Dictionary<string, string> KeywordArgs { get; }

    public UrlNode(string viewName, List<string> positionalArgs, Dictionary<string, string> keywordArgs)
    {
        ViewName = viewName;
        PositionalArgs = positionalArgs;
        KeywordArgs = keywordArgs;
    }

    public override string Render(Dictionary<string, object> context)
    {
        return ReverseUrl(ViewName, KeywordArgs.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value))!;
    }
}




public static class UrlTag
{
    public static BaseNode ParseUrlTag(Parser parser, Token token)
    {
        // Split token.Content into parts (assumes proper spacing).
        // Example token.Content: "url 'user-detail' id=42" or "url 'home'"
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !parts[0].Equals("url", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Invalid url tag syntax. Expected: {% url 'view_name' [args] %}");
        }
        
        // The view name should be in parts[1]. Remove surrounding quotes if present.
        string viewName = parts[1].Trim();
        if ((viewName.StartsWith("\"") && viewName.EndsWith("\"")) ||
            (viewName.StartsWith("'") && viewName.EndsWith("'")))
        {
            viewName = viewName.Substring(1, viewName.Length - 2);
        }
        
        // Prepare containers for positional and keyword arguments.
        var positionalArgs = new List<string>();
        var keywordArgs = new Dictionary<string, string>();

        // Process any remaining parts.
        for (int i = 2; i < parts.Length; i++)
        {
            string tokenPart = parts[i].Trim();

            // Check for keyword argument (contains '=')
            if (tokenPart.Contains("="))
            {
                var kv = tokenPart.Split(new[] { '=' }, 2);
                if (kv.Length != 2)
                    throw new Exception("Invalid keyword argument in url tag: " + tokenPart);
                string key = kv[0].Trim();
                string val = kv[1].Trim();
                // Remove quotes from the value if present.
                if ((val.StartsWith("\"") && val.EndsWith("\"")) ||
                    (val.StartsWith("'") && val.EndsWith("'")))
                {
                    val = val.Substring(1, val.Length - 2);
                }
                keywordArgs[key] = val;
            }
            else
            {
                positionalArgs.Add(tokenPart);
            }
        }
        
        return new UrlNode(viewName, positionalArgs, keywordArgs);
    }
}
