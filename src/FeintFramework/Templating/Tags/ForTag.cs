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
        var sequenceObj = VariableNode.GetVariableValue(IterableName, context);

        if (!(sequenceObj is IEnumerable sequence))
            return "";

        var output = new StringBuilder();
        
        
        var length = -1;
        if (sequenceObj is ICollection collection)
        {
            length = collection.Count;
        }
        else if (sequenceObj is Array array)
        {
            length = array.Length;
        }
        else if (sequenceObj is IList list)
        {
            length = list.Count;
        }
        var index = 0;
        var parentLoop = context.ContainsKey("forloop") ? (Dictionary<string, object>)context["forloop"] : null;
        foreach (var item in sequence)
        {
            var localContext = new Dictionary<string, object>(context)
            {
                [LoopVariable] = item,
                ["forloop"] = new Dictionary<string, object>
                {
                    { "counter", index + 1 },
                    { "counter0", index },
                    { "revcounter", length - index },
                    { "revcounter0", length - index - 1 },
                    { "length", length },
                    { "first", index == 0 },
                    { "last", index == length - 1 },
                    { "parentloop", parentLoop }
                }

            };

            foreach (var child in Children)
            {
                output.Append(child.Render(localContext));
            }
            index++; // increment index after each iteration
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