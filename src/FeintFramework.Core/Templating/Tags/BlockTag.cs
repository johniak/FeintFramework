using FeintFramework.Core.Templating.Node;

namespace FeintFramework.Core.Templating.Tags;


public class BlockTagNode : BaseNode
{
    public string Name { get; }
    public List<BaseNode> Children { get; } = new List<BaseNode>();

    public BlockTagNode? ParentBlock { get; set; }

    public BlockTagNode(string name)
    {
        Name = name;
    }

    protected string RenderChildren(Dictionary<string, object> context)
    {
        return string.Join("", Children.Select(child => child.Render(context)));
    }

    public override string Render(Dictionary<string, object> context)
    {
        if(context.TryGetValue("childblock_" + Name, out object childObj) && childObj is BlockTagNode childBlock){
            var localContext = new Dictionary<string, object>(context);
            localContext["block"] = new { super = RenderChildren(context) };
            return childBlock.RenderChildren(localContext);
        }
        return RenderChildren(context);
    }
}


public static class BlockTag
{
    public static BaseNode ParseBlockTag(Parser parser, Token token)
    {
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || parts[0].ToLowerInvariant() != "block")
        {
            throw new Exception("Invalid block tag syntax. Expected: {% block blockName %}");
        }
        string blockName = parts[1].Trim();
        BlockTagNode blockTagNode = new BlockTagNode(blockName);
        while (!parser.IsAtEnd() && !(parser.Peek().Type == TokenType.Block &&
               parser.Peek().Content.Trim().ToLowerInvariant().StartsWith("endblock")))
        {
            blockTagNode.Children.Add(parser.ParseNode());
        }
        if (parser.IsAtEnd())
        {
            throw new Exception("Missing endblock for block: " + blockName);
        }
        parser.Advance();
        return blockTagNode;
    }
}