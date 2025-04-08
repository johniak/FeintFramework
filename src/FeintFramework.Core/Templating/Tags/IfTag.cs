using FeintFramework.Core.Templating.Node;

namespace FeintFramework.Core.Templating.Tags;

public enum ConditionTokenType
{
    Identifier,
    Number,
    StringLiteral,
    Operator,
    And,
    Or,
    End
}

public class ConditionToken
{
    public ConditionTokenType Type { get; }
    public string Value { get; }

    public ConditionToken(ConditionTokenType type, string value)
    {
        Type = type;
        Value = value;
    }
}

public class ConditionParser
{
    private readonly List<ConditionToken> tokens;
    private int index;
    private readonly Dictionary<string, object> context;

    public ConditionParser(List<ConditionToken> tokens, Dictionary<string, object> context)
    {
        this.tokens = tokens;
        this.context = context;
        index = 0;
    }

    private ConditionToken Current => tokens[index];

    private void NextToken() => index++;

    public bool ParseExpression()
    {
        return ParseOrExpression();
    }

    private bool ParseOrExpression()
    {
        bool left = ParseAndExpression();
        while (Current.Type == ConditionTokenType.Or)
        {
            NextToken();
            bool right = ParseAndExpression();
            left = left || right;
        }
        return left;
    }

    private bool ParseAndExpression()
    {
        bool left = ParseComparison();
        while (Current.Type == ConditionTokenType.And)
        {
            NextToken();
            bool right = ParseComparison();
            left = left && right;
        }
        return left;
    }

    private bool ParseComparison()
    {
        var leftValue = ParsePrimary();
        if (Current.Type == ConditionTokenType.Operator)
        {
            string op = Current.Value;
            NextToken();
            var rightValue = ParsePrimary();
            return Compare(leftValue, rightValue, op);
        }
        return ToBool(leftValue);
    }


    private object ParsePrimary()
    {
        ConditionToken token = Current;
        NextToken();
        switch (token.Type)
        {
            case ConditionTokenType.Number:
                if (int.TryParse(token.Value, out int intValue))
                    return intValue;
                return token.Value;
            case ConditionTokenType.StringLiteral:
                return token.Value.Trim('\'', '"');
            case ConditionTokenType.Identifier:
                return VariableNode.GetVariableValue(token.Value, context);
            default:
                return token.Value;
        }
    }

    private bool ToBool(object value)
    {
        if (value is bool b)
            return b;
        if (value is int i)
            return i != 0;
        if (value is string s)
            return !string.IsNullOrEmpty(s);
        return value != null;
    }

    private bool Compare(object left, object right, string op)
    {
        if (left == null || right == null) return false;
        try
        {
            if (double.TryParse(left.ToString(), out double leftNum) &&
                double.TryParse(right.ToString(), out double rightNum))
            {
                switch (op)
                {
                    case "==": return leftNum == rightNum;
                    case "!=": return leftNum != rightNum;
                    case "<": return leftNum < rightNum;
                    case "<=": return leftNum <= rightNum;
                    case ">": return leftNum > rightNum;
                    case ">=": return leftNum >= rightNum;
                    default: return false;
                }
            }
            else
            {
                int cmp = string.Compare(left.ToString(), right.ToString());
                switch (op)
                {
                    case "==": return cmp == 0;
                    case "!=": return cmp != 0;
                    case "<": return cmp < 0;
                    case "<=": return cmp <= 0;
                    case ">": return cmp > 0;
                    case ">=": return cmp >= 0;
                    default: return false;
                }
            }
        }
        catch
        {
            return false;
        }
    }
}

public class IfNode : BaseNode
{
    public string Condition { get; }
    public List<BaseNode> TrueChildren { get; } = new List<BaseNode>();
    public List<BaseNode> FalseChildren { get; } = new List<BaseNode>();

    public IfNode(string condition)
    {
        Condition = condition;
    }

    private bool EvaluateCondition(Dictionary<string, object> context)
    {
        var tokens = TokenizeCondition(Condition);
        var parser = new ConditionParser(tokens, context);
        return parser.ParseExpression();
    }

    private static List<ConditionToken> TokenizeCondition(string condition)
    {
        var tokens = new List<ConditionToken>();
        var parts = condition.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (part == "and")
                tokens.Add(new ConditionToken(ConditionTokenType.And, part));
            else if (part == "or")
                tokens.Add(new ConditionToken(ConditionTokenType.Or, part));
            else if (part == "==" || part == "!=" || part == "<" || part == "<=" || part == ">" || part == ">=")
                tokens.Add(new ConditionToken(ConditionTokenType.Operator, part));
            else if ((part.StartsWith("\"") && part.EndsWith("\"")) ||
                     (part.StartsWith("'") && part.EndsWith("'")))
                tokens.Add(new ConditionToken(ConditionTokenType.StringLiteral, part));
            else if (int.TryParse(part, out _))
                tokens.Add(new ConditionToken(ConditionTokenType.Number, part));
            else
                tokens.Add(new ConditionToken(ConditionTokenType.Identifier, part));
        }

        tokens.Add(new ConditionToken(ConditionTokenType.End, string.Empty));
        return tokens;
    }

    public override string Render(Dictionary<string, object> context)
    {
        if (EvaluateCondition(context))
        {
            return string.Join("", TrueChildren.Select(child => child.Render(context)));
        }
        else
        {
            return string.Join("", FalseChildren.Select(child => child.Render(context)));
        }
    }
}

public static class IfTag
{
    public static BaseNode ParseIfTag(Parser parser, Token token)
    {
        var content = token.Content.Trim();

        var condition = content.Substring(2).Trim();
        var ifNode = new IfNode(condition);

        bool inElseBlock = false;

        while (!parser.IsAtEnd())
        {
            var current = parser.Peek();

            if (current.Type == TokenType.Block)
            {
                var blockContent = current.Content.Trim().ToLowerInvariant();

                if (blockContent == "endif")
                {
                    parser.Advance();
                    return ifNode;
                }
                else if (blockContent == "else")
                {
                    parser.Advance();
                    inElseBlock = true;
                    continue;
                }
            }

            var node = parser.ParseNode();
            if (!inElseBlock)
            {
                ifNode.TrueChildren.Add(node);
            }
            else
            {
                ifNode.FalseChildren.Add(node);
            }
        }

        throw new Exception("Missing endif tag.");
    }
}