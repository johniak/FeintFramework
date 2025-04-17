
using FeintFramework.Templating.Node;

namespace FeintFramework.Templating.Tags;

public class ExtendsNode : BaseNode
{
    public string ParentTemplateName { get; }
    public Dictionary<string, BlockTagNode> ChildBlocks { get; }

    public ExtendsNode(string parentTemplateName, Dictionary<string, BlockTagNode> childBlocks)
    {
        ParentTemplateName = parentTemplateName;
        ChildBlocks = childBlocks;
    }

    public override string Render(Dictionary<string, object> context)
    {
        TemplateNode parentTemplate = TemplateLoader.Load(ParentTemplateName);
        foreach (var block in ChildBlocks)
        {
            context["childblock_" + block.Key] = block.Value;
        }
        return parentTemplate.Render(context);
    }
}

public static class ExtendsTag
{
    public static BaseNode ParseExtendsTag(Parser parser, Token token)
    {
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || parts[0].ToLowerInvariant() != "extends")
        {
            throw new Exception("Invalid extends tag syntax. Expected: {% extends \"parent.html\" %}");
        }
        string parentTemplateName = parts[1].Trim().Trim('\'', '"');

        Dictionary<string, BlockTagNode> childBlocks = new Dictionary<string, BlockTagNode>();
        while (!parser.IsAtEnd())
        {
            BaseNode node = parser.ParseNode();
            if (node is BlockTagNode blockNode)
            {
                childBlocks[blockNode.Name] = blockNode;
            }
        }
        return new ExtendsNode(parentTemplateName, childBlocks);
    }
}
