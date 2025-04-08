namespace FeintFramework.Core.Templating.Node;
    
public class TextNode : BaseNode
{
    public string Text { get; }
    public TextNode(string text) => Text = text;
    public override string Render(Dictionary<string, object> context) => Text;
}