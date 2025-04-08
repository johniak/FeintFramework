
using FeintFramework.Core.Templating.Node;

namespace FeintFramework.Core.Templating;
public delegate BaseNode TagParseHandler(Parser parser, Token token);

public static class TagRegistry
{
    private static readonly Dictionary<string, TagParseHandler> _registry = new Dictionary<string, TagParseHandler>();

    public static void Register(string tagName, TagParseHandler handler)
    {
        _registry[tagName.ToLowerInvariant()] = handler;
    }

    public static bool TryGetHandler(string tagName, out TagParseHandler handler)
    {
        return _registry.TryGetValue(tagName.ToLowerInvariant(), out handler);
    }
}