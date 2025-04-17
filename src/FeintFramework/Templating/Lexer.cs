namespace FeintFramework.Templating;


public enum TokenType
{
    Text,
    Variable,
    Block,
    Comment
}
public class TokenInfo
{
    public required string TokenStart { get; set; }
    public required string TokenEnd { get; set; }
    public required TokenType Type { get; set; }
}

public class Token
{
    public TokenType Type { get; }
    public string Content { get; }
    public int Start { get; }
    public int End { get; }

    public Token(TokenType type, string content, int start, int end)
    {
        Type = type;
        Content = content;
        Start = start;
        End = end;
    }

    public override string ToString() => $"{Type} (from {Start} to {End}): {Content}";
}

public class Lexer
{
    public List<TokenInfo> TokensInfo = new List<TokenInfo>(){
        {new TokenInfo(){TokenStart="{{", TokenEnd="}}",Type=TokenType.Variable}},
        {new TokenInfo(){TokenStart="{%", TokenEnd="%}",Type=TokenType.Block}},
        {new TokenInfo(){TokenStart="{#", TokenEnd="#}",Type=TokenType.Comment}},
    };
    public List<Token> Tokenize(string template)
    {
        List<Token> tokens = new List<Token>();
        int pos = 0;

        while (pos < template.Length)
        {
            var nextTokens = TokensInfo.Select(t => (TokenIndex: template.IndexOf(t.TokenStart, pos), TokenInfo: t));

            var nextTag = FindNextTagPosition(nextTokens);
            if (!nextTag.HasValue)
            {
                // no tokens to the end of the file
                tokens.Add(new Token(TokenType.Text, template.Substring(pos), pos, template.Length));
                break;
            }
            var nextTagPosition = nextTag.Value.TokenIndex;
            var nextTagInfo = nextTag.Value.TokenInfo;
            if (nextTagPosition > pos)
            {
                // add text between tags
                tokens.Add(new Token(TokenType.Text, template.Substring(pos, nextTagPosition - pos), pos, nextTagPosition));
            }
            int endTag = template.IndexOf(nextTagInfo.TokenEnd, nextTagPosition);
            if (endTag == -1)
                throw new Exception($"Closing tag not found for {nextTagInfo.Type} token.");
            int contentStart = nextTagPosition + nextTagInfo.TokenStart.Length;
            int contentLength = endTag - contentStart;
            string content = template.Substring(contentStart, contentLength).Trim();
            tokens.Add(new Token(nextTagInfo.Type, content, nextTagPosition, endTag + nextTagInfo.TokenEnd.Length));
            pos = endTag + nextTagInfo.TokenEnd.Length;
        }
        return tokens;
    }

    private (int TokenIndex, TokenInfo TokenInfo)? FindNextTagPosition(IEnumerable<(int TokenIndex, TokenInfo TokenInfo)> positions)
    {
        int min = int.MaxValue;
        TokenInfo? nextToken = null;
        foreach (var pos in positions)
        {
            if (pos.TokenIndex != -1 && pos.TokenIndex < min)
            {
                min = pos.TokenIndex;
                nextToken = pos.TokenInfo;
            }
        }
        if (nextToken == null)
        {
            return null;
        }
        return (min, nextToken);
    }
}
