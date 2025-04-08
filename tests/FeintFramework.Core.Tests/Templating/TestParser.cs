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

    [TestMethod]
    public void VariableNode_SimpleVariable_RendersCorrectValue()
    {
        string template = "Hello, {{ name }}!";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "name", "World" }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Hello, World!", output);
    }

    [TestMethod]
    public void VariableNode_NestedProperty_RendersCorrectValue()
    {
        string template = "User: {{ user.name }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "user", new { name = "Alice" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("User: Alice", output);
    }

    [TestMethod]
    public void VariableNode_ListIndex_RendersCorrectValue()
    {
        string template = "Item: {{ items.1 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "apple", "banana", "cherry" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Item: banana", output);
    }

    [TestMethod]
    public void VariableNode_InvalidVariable_ReturnsEmptyString()
    {
        string template = "Unknown: {{ unknown }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>();

        string output = ast.Render(context);
        Assert.AreEqual("Unknown: ", output);
    }

    [TestMethod]
    public void VariableNode_DictionaryAccess_RendersCorrectValue()
    {
        string template = "Value: {{ data.key }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "data", new Dictionary<string, object> { { "key", "value" } } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Value: value", output);
    }

    [TestMethod]
    public void VariableNode_IEnumerableAccess_RendersCorrectValue()
    {
        string template = "Item: {{ items.1 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "apple", "banana", "cherry" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Item: banana", output);
    }

    [TestMethod]
    public void VariableNode_IEnumerableIndex_RendersCorrectValue()
    {
        string template = "Item: {{ items.2 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", (IEnumerable<string>)new HashSet<string> { "apple", "banana", "cherry" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Item: cherry", output);
    }

    [TestMethod]
    public void VariableNode_InvalidIndex_ReturnsEmptyString()
    {
        string template = "Item: {{ items.10 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "apple", "banana", "cherry" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Item: ", output);
    }

    [TestMethod]
    public void VariableNode_FieldAccess_RendersCorrectValue()
    {
        string template = "Field: {{ obj.field }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        var obj = new TestObjectWithField { field = "fieldValue" };
        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "obj", obj }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Field: fieldValue", output);
    }

    [TestMethod]
    public void VariableNode_ListAccess_RendersCorrectValue()
    {
        string template = "Item: {{ items.1 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", new List<string> { "apple", "banana", "cherry" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Item: banana", output);
    }

    [TestMethod]
    public void VariableNode_InvalidEnumerableIndex_ReturnsEmptyString()
    {
        string template = "Item: {{ items.10 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", (IEnumerable<string>)new HashSet<string> { "apple", "banana", "cherry" } }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Item: ", output);
    }

    [TestMethod]
    public void VariableNode_NonEnumerableIndex_ReturnsEmptyString()
    {
        string template = "Value: {{ obj.1 }}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "obj", new object() }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Value: ", output);
    }

    [TestMethod]
    public void ParserAndRenderer_CommentNode_IgnoresComment()
    {
        string template = "Hello, World! {# This is a comment #} Goodbye, World!";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>();

        string output = ast.Render(context);
        Assert.AreEqual("Hello, World!  Goodbye, World!", output);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Missing endblock for block: testBlock")]
    public void ParseBlockTag_ThrowsException_WhenEndblockIsMissing()
    {
        string template = "{% block testBlock %} Content without endblock";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);

        BlockTag.ParseBlockTag(parser, tokens[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Invalid block tag syntax. Expected: {% block blockName %}")]
    public void ParseBlockTag_ThrowsException_WhenSyntaxIsInvalid()
    {
        string template = "{% block %}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);

        BlockTag.ParseBlockTag(parser, tokens[0]);
    }

    [TestMethod]
    public void ForNode_Render_ReturnsEmptyString_WhenIterableNotInContext()
    {
        string template = "{% for item in items %}{{ item }}{% endfor %}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>();

        string output = ast.Render(context);
        Assert.AreEqual("", output);
    }

    [TestMethod]
    public void ForNode_Render_ReturnsEmptyString_WhenIterableIsNotEnumerable()
    {
        string template = "{% for item in items %}{{ item }}{% endfor %}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "items", 123 } // Not an IEnumerable
        };

        string output = ast.Render(context);
        Assert.AreEqual("", output);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "Invalid syntax. {% for <item> in <collection> %} expected.")]
    public void ForTag_ParseForTag_ThrowsException_WhenSyntaxIsInvalid()
    {
        string template = "{% for item items %}{{ item }}{% endfor %}"; // Missing 'in'

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);

        ForTag.ParseForTag(parser, tokens[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), "'endfor' tag not found.")]
    public void ForTag_ParseForTag_ThrowsException_WhenEndforIsMissing()
    {
        string template = "{% for item in items %}{{ item }}"; // Missing 'endfor'

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);

        ForTag.ParseForTag(parser, tokens[0]);
    }

    [TestMethod]
    public void ParserAndRenderer_IfCondition_ParsesAndRendersCorrectly()
    {
        string template = "{% if x == 5 %}Equal{% else %}Not Equal{% endif %}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();

        Dictionary<string, object> context = new Dictionary<string, object>
        {
            { "x", 5 }
        };

        string output = ast.Render(context);
        Assert.AreEqual("Equal", output);

        context["x"] = 10;
        output = ast.Render(context);
        Assert.AreEqual("Not Equal", output);
    }

    [TestMethod]
    public void ParserAndRenderer_IfCondition_ComparisonOperators()
    {
        string template = "{% if x == 5 %}Equal{% else %}Not Equal{% endif %}";
        Assert.AreEqual("Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", 5 } }));
        Assert.AreEqual("Not Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", 10 } }));

        template = "{% if x != 5 %}Not Equal{% else %}Equal{% endif %}";
        Assert.AreEqual("Not Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", 10 } }));
        Assert.AreEqual("Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", 5 } }));

        template = "{% if x < 5 %}Less{% else %}Not Less{% endif %}";
        Assert.AreEqual("Less", RenderTemplate(template, new Dictionary<string, object> { { "x", 3 } }));
        Assert.AreEqual("Not Less", RenderTemplate(template, new Dictionary<string, object> { { "x", 5 } }));

        template = "{% if x <= 5 %}Less or Equal{% else %}Greater{% endif %}";
        Assert.AreEqual("Less or Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", 5 } }));
        Assert.AreEqual("Greater", RenderTemplate(template, new Dictionary<string, object> { { "x", 6 } }));

        template = "{% if x > 5 %}Greater{% else %}Not Greater{% endif %}";
        Assert.AreEqual("Greater", RenderTemplate(template, new Dictionary<string, object> { { "x", 6 } }));
        Assert.AreEqual("Not Greater", RenderTemplate(template, new Dictionary<string, object> { { "x", 5 } }));

        template = "{% if x >= 5 %}Greater or Equal{% else %}Less{% endif %}";
        Assert.AreEqual("Greater or Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", 5 } }));
        Assert.AreEqual("Less", RenderTemplate(template, new Dictionary<string, object> { { "x", 4 } }));

        template = "{% if x == 'apple' %}Equal{% else %}Not Equal{% endif %}";
        Assert.AreEqual("Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", "apple" } }));
        Assert.AreEqual("Not Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", "banana" } }));

        template = "{% if x != 'apple' %}Not Equal{% else %}Equal{% endif %}";
        Assert.AreEqual("Not Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", "banana" } }));
        Assert.AreEqual("Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", "apple" } }));

        template = "{% if x < 'banana' %}Less{% else %}Not Less{% endif %}";
        Assert.AreEqual("Less", RenderTemplate(template, new Dictionary<string, object> { { "x", "apple" } }));
        Assert.AreEqual("Not Less", RenderTemplate(template, new Dictionary<string, object> { { "x", "banana" } }));

        template = "{% if x <= 'banana' %}Less or Equal{% else %}Greater{% endif %}";
        Assert.AreEqual("Less or Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", "banana" } }));
        Assert.AreEqual("Greater", RenderTemplate(template, new Dictionary<string, object> { { "x", "cherry" } }));

        template = "{% if x > 'apple' %}Greater{% else %}Not Greater{% endif %}";
        Assert.AreEqual("Greater", RenderTemplate(template, new Dictionary<string, object> { { "x", "banana" } }));
        Assert.AreEqual("Not Greater", RenderTemplate(template, new Dictionary<string, object> { { "x", "apple" } }));

        template = "{% if x >= 'banana' %}Greater or Equal{% else %}Less{% endif %}";
        Assert.AreEqual("Greater or Equal", RenderTemplate(template, new Dictionary<string, object> { { "x", "banana" } }));
        Assert.AreEqual("Less", RenderTemplate(template, new Dictionary<string, object> { { "x", "apple" } }));
    }

    private string RenderTemplate(string template, Dictionary<string, object> context)
    {
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();
        return ast.Render(context);
    }

    private class TestObjectWithField
    {
        public string field;
    }

    private class TestObject
    {
        public string Field;
    }
}