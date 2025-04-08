namespace FeintFramework.Core.Templating.Node;

public class CommentNode : BaseNode
{
    public string Comment { get; }
    public CommentNode(string comment) => Comment = comment;
    public override string Render(Dictionary<string, object> context) => ""; 
}