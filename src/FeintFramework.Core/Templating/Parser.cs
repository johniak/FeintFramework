using FeintFramework.Core.Templating.Node;

namespace FeintFramework.Core.Templating;

public partial class Parser
{
    private readonly List<Token> tokens;
    private int position = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }


    public TemplateNode ParseTemplate()
    {
        TemplateNode templateNode = new TemplateNode();
        while (!IsAtEnd())
        {
            BaseNode node = ParseNode();
            if (node != null)
            {
                templateNode.Children.Add(node);
            }
        }
        return templateNode;
    }


    public BaseNode ParseNode()
    {
        var token = Peek();
        switch (token.Type)
        {
            case TokenType.Text:
                Advance();
                return new TextNode(token.Content);
            case TokenType.Variable:
                Advance();
                return new VariableNode(token.Content);
            case TokenType.Comment:
                Advance();
                return new CommentNode();
            case TokenType.Block:
                return ParseBlock();
            default:
                throw new Exception("Nieznany typ tokena.");
        }
    }


    private BaseNode ParseBlock()
    {
        Token token = Advance();
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            string tagName = parts[0].ToLowerInvariant();
            if (TagRegistry.TryGetHandler(tagName, out TagParseHandler handler))
            {
                return handler(this, token);
            }
        }
        throw new Exception($"Unknown tag: {token.Content}");
    }

    public Token Peek() => tokens[position];

    public Token Advance() => tokens[position++];

    public bool IsAtEnd() => position >= tokens.Count;
}