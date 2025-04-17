

using System.Collections;
using System.Text;
using FeintFramework.Templating.Node;
namespace FeintFramework.Templating.Tags;

public class ForNode : BaseNode
{
    public string LoopVariable { get; }
    public string IterableName { get; }
    public List<BaseNode> Children { get; }

    public ForNode(string loopVariable, string iterableName, List<BaseNode> children)
    {
        LoopVariable = loopVariable;
        IterableName = iterableName;
        Children = children;
    }

    public override string Render(Dictionary<string, object> context)
    {
        var collectionObj = VariableNode.GetVariableValue(IterableName, context);

        if (!(collectionObj is IEnumerable collection))
            return "";

        var output = new StringBuilder();

        foreach (var item in collection)
        {
            // Tworzymy lokalny kontekst dla każdej iteracji
            var localContext = new Dictionary<string, object>(context)
            {
                [LoopVariable] = item
            };

            foreach (var child in Children)
            {
                output.Append(child.Render(localContext));
            }
        }

        return output.ToString();
    }
}


public static class ForTag
{
    public static BaseNode ParseForTag(Parser parser, Token token)
    {
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 4 || parts[0].ToLowerInvariant() != "for" || parts[2].ToLowerInvariant() != "in")
        {
            throw new Exception("Invalid syntax. {% for <item> in <collection> %} expected.");
        }
        string loopVariable = parts[1];
        string iterableName = parts[3];

        List<BaseNode> children = new List<BaseNode>();
        while (!parser.IsAtEnd() && !(parser.Peek().Type == TokenType.Block && parser.Peek().Content.Trim().ToLowerInvariant() == "endfor"))
        {
            children.Add(parser.ParseNode());
        }
        if (parser.IsAtEnd())
            throw new Exception("'endfor' tag not found.");
        parser.Advance();

        return new ForNode(loopVariable, iterableName, children);
    }
}