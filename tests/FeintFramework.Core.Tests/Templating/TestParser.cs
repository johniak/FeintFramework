using FeintFramework.Core.Templating.Node;
using FeintFramework.Core.Templating.Tags;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework.Utilities;

namespace FeintFramework.Core.Templating;

[TestClass]
public sealed class TestParser
{
    [TestMethod]
    public void ParserAndRenderer_ForLoop_RendersText()
    {
        TagRegistry.Register("for", ForTag.ParseForTag);
        string template = "Witaj,{% for item in items %}{{ item }} {% endfor %}Do zobaczenia!";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "jabłko", "banan", "wiśnia" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Witaj,jabłko banan wiśnia Do zobaczenia!", output);
    }

    [TestMethod]
    public void ParserAndRenderer_NestedVariable_RendersText()
    {
        TagRegistry.Register("for", ForTag.ParseForTag);
        string template = "Test nested {{ items.Count }} {{items.2}}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "jabłko", "banan", "wiśnia" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Test nested 3 wiśnia", output);
    }

    [TestMethod]
    public void ParserAndRenderer_IncludeTag_RendersText()
    {
        TagRegistry.Register("for", ForTag.ParseForTag);
        TagRegistry.Register("include", IncludeTag.ParseIncludeTag);
        string template = "Test nested {{ items.Count }} {% include 'Templating/data/include.html'%}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "jabłko", "banan", "wiśnia" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Test nested 3 Included", output);
    }

    [TestMethod]
    public void ParserAndRenderer_TestInheritance_RendersText()
    {
        TagRegistry.Register("for", ForTag.ParseForTag);
        TagRegistry.Register("include", IncludeTag.ParseIncludeTag);
        TagRegistry.Register("extends", ExtendsTag.ParseExtendsTag);
        TagRegistry.Register("block", BlockTag.ParseBlockTag);
        string template = "{% extends \"Templating/data/base.html\" %}{%block foo%}childblock {{items.Count}}{%endblock%}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "jabłko", "banan", "wiśnia" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Base Template childblock 3", output);
    }
        [TestMethod]
    public void ParserAndRenderer_TestInheritance_RendersSuperBlock()
    {
        TagRegistry.Register("for", ForTag.ParseForTag);
        TagRegistry.Register("include", IncludeTag.ParseIncludeTag);
        TagRegistry.Register("extends", ExtendsTag.ParseExtendsTag);
        TagRegistry.Register("block", BlockTag.ParseBlockTag);
        string template = "{% extends \"Templating/data/base.html\" %}{%block foo%}childblock {{items.Count}} {{block.super}}{%endblock%}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "jabłko", "banan", "wiśnia" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Base Template childblock 3 baseblock", output);
    }
}