using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework.Utilities;

namespace FeintFramework.Templating;

[TestClass]
public sealed class TestLexer
{

    [TestMethod]
    public void Tokenize_CreatesTokens_WhenOnlyText()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(" Just Text ");
        Assert.AreEqual(tokens.Count, 1);
        var token = tokens[0];
        Assert.AreEqual(TokenType.Text, token.Type);
        Assert.AreEqual(" Just Text ", token.Content);
    }

    [TestMethod]
    public void Tokenize_CreatesTokens_WhenVariable()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("{{foo}}");
        Assert.AreEqual(tokens.Count, 1);
        var token = tokens[0];
        Assert.AreEqual(TokenType.Variable, token.Type);
        Assert.AreEqual("foo", token.Content);
    }
    [TestMethod]
    public void Tokenize_CreatesTokens_WhenVariableWithWhitespace()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("{{ foo }}");
        Assert.AreEqual(tokens.Count, 1);
        var token = tokens[0];
        Assert.AreEqual(TokenType.Variable, token.Type);
        Assert.AreEqual("foo", token.Content);
    }

    [TestMethod]
    public void Tokenize_CreatesTokens_WhenBlock()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("{% for x in x_list %}");
        Assert.AreEqual(tokens.Count, 1);
        var token = tokens[0];
        Assert.AreEqual(TokenType.Block, token.Type);
        Assert.AreEqual("for x in x_list", token.Content);
    }

    [TestMethod]
    public void Tokenize_CreatesTokens_WhenComment()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("{# Test Comment #}");
        Assert.AreEqual(tokens.Count, 1);
        var token = tokens[0];
        Assert.AreEqual(TokenType.Comment, token.Type);
        Assert.AreEqual("Test Comment", token.Content);
    }

    [TestMethod]
    public void Tokenize_CreatesTokens_WhenTextBeetweenOtherTokens()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("{{foo}} Just Text {{bar}}");
        Assert.AreEqual(tokens.Count, 3);
        var fooToken = tokens[0];
        var textToken = tokens[1];
        var barToken = tokens[2];

        Assert.AreEqual(TokenType.Variable, fooToken.Type);
        Assert.AreEqual("foo", fooToken.Content);

        Assert.AreEqual(TokenType.Text, textToken.Type);
        Assert.AreEqual(" Just Text ", textToken.Content);

        Assert.AreEqual(TokenType.Variable, barToken.Type);
        Assert.AreEqual("bar", barToken.Content);
    }

    [TestMethod]
    public void Tokenize_CreatesTokens_WithDifferentTypes()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("{{var}} {% block %} {# comment #}");
        Assert.AreEqual(5, tokens.Count);
        
        Assert.AreEqual(TokenType.Variable, tokens[0].Type);
        Assert.AreEqual("var", tokens[0].Content);
        
        Assert.AreEqual(TokenType.Text, tokens[1].Type);
        Assert.AreEqual(" ", tokens[1].Content);
        
        Assert.AreEqual(TokenType.Block, tokens[2].Type);
        Assert.AreEqual("block", tokens[2].Content);
        
        Assert.AreEqual(TokenType.Text, tokens[3].Type);
        Assert.AreEqual(" ", tokens[3].Content);
        
        Assert.AreEqual(TokenType.Comment, tokens[4].Type);
        Assert.AreEqual("comment", tokens[4].Content);
    }
    
    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Tokenize_ThrowsException_WhenUnclosedTag()
    {
        var lexer = new Lexer();
        lexer.Tokenize("{{unclosed");
    }
    
    [TestMethod]
    public void Tokenize_TrackPosition_Correctly()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("text{{var}}");
        
        Assert.AreEqual(2, tokens.Count);
        
        Assert.AreEqual(0, tokens[0].Start);
        Assert.AreEqual(4, tokens[0].End);
        
        Assert.AreEqual(4, tokens[1].Start);
        Assert.AreEqual(11, tokens[1].End);
    }
    
    [TestMethod]
    public void Tokenize_HandlesNestedTokens_Correctly()
    {
        var lexer = new Lexer();
        // Note: This doesn't actually nest tokens - it's testing sequential tokens with similar prefixes
        var tokens = lexer.Tokenize("{# Comment with {{ inside }} #}{{ Variable with {% inside %} }}");
        
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(TokenType.Comment, tokens[0].Type);
        Assert.AreEqual("Comment with {{ inside }}", tokens[0].Content);
        
        Assert.AreEqual(TokenType.Variable, tokens[1].Type);
        Assert.AreEqual("Variable with {% inside %}", tokens[1].Content);
    }
    
    [TestMethod]
    public void Tokenize_HandlesEmptyTemplate_Correctly()
    {
        var lexer = new Lexer();
        var tokens = lexer.Tokenize("");
        
        Assert.AreEqual(0, tokens.Count);
    }
}