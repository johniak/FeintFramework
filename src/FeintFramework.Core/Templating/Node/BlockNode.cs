namespace FeintFramework.Core.Templating.Node;

public class BlockNode : BaseNode
{
    public string TagContent { get; }
    public List<BaseNode> Children { get; }
    public BlockNode(string tagContent)
    {
        TagContent = tagContent;
        Children = new List<BaseNode>();
    }
    public override string Render(Dictionary<string, object> context)
    {
        return string.Join("", Children.Select(child => child.Render(context)));
    }
}