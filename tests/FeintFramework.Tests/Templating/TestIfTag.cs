using FeintFramework.Templating.Tags;
using FeintFramework.Templating.Node;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FeintFramework.Templating;

[TestClass]
public sealed class TestIfTag
{
    [TestInitialize]
    public void Initialize()
    {
        // Register the if tag handler
        TagRegistry.Register("if", IfTag.ParseIfTag);
    }

    [TestMethod]
    public void If_WithTrueCondition_RendersIfBlock()
    {
        // Arrange
        string template = "{% if isActive %}Active{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "isActive", true }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Active", result);
    }

    [TestMethod]
    public void If_WithFalseCondition_RendersEmpty()
    {
        // Arrange
        string template = "{% if isActive %}Active{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "isActive", false }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void IfElse_WithTrueCondition_RendersIfBlock()
    {
        // Arrange
        string template = "{% if isActive %}Active{% else %}Inactive{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "isActive", true }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Active", result);
    }

    [TestMethod]
    public void IfElse_WithFalseCondition_RendersElseBlock()
    {
        // Arrange
        string template = "{% if isActive %}Active{% else %}Inactive{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "isActive", false }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Inactive", result);
    }

    [TestMethod]
    public void If_WithMissingVariable_RendersElseBlock()
    {
        // Arrange
        string template = "{% if nonExistent %}Exists{% else %}Missing{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>();

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Missing", result);
    }

    [TestMethod]
    public void If_WithEqualityComparison_RendersCorrectly()
    {
        // Arrange
        string template = "{% if count == 5 %}Equal{% else %}Not Equal{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "count", 5 }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Equal", result);
    }

    [TestMethod]
    public void If_WithInequalityComparison_RendersCorrectly()
    {
        // Arrange
        string template = "{% if count != 10 %}Not Equal{% else %}Equal{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "count", 5 }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Not Equal", result);
    }

    [TestMethod]
    public void If_WithGreaterThanComparison_RendersCorrectly()
    {
        // Arrange
        string template = "{% if count > 3 %}Greater{% else %}Not Greater{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "count", 5 }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Greater", result);
    }

    [TestMethod]
    public void If_WithLessThanComparison_RendersCorrectly()
    {
        // Arrange
        string template = "{% if count < 10 %}Less{% else %}Not Less{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "count", 5 }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Less", result);
    }

    [TestMethod]
    public void If_WithNestedProperty_RendersCorrectly()
    {
        // Arrange
        string template = "{% if user.IsActive %}Active User{% else %}Inactive User{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "user", new UserTestModel { IsActive = true } }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Active User", result);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void If_WithMissingEndIf_ThrowsException()
    {
        // Arrange
        string template = "{% if isActive %}Active";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        
        // Act - should throw exception
        parser.ParseTemplate();
    }

    [TestMethod]
    public void If_WithStringLiteral_RendersCorrectly()
    {
        // Arrange
        string template = "{% if role == 'admin' %}Admin Access{% else %}User Access{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "role", "admin" }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Admin Access", result);
    }

    [TestMethod]
    public void If_WithNestedIf_RendersCorrectly()
    {
        // Arrange
        string template = "{% if isActive %}{% if isAdmin %}Admin{% else %}User{% endif %}{% else %}Inactive{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();
        
        var context = new Dictionary<string, object>
        {
            { "isActive", true },
            { "isAdmin", true }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Admin", result);
    }

    [TestMethod]
    public void If_WithComplexCondition_RendersCorrectly()
    {
        string template = "{% if count > 5 and count < 10 %}Within Range{% else %}Out of Range{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();

        var context = new Dictionary<string, object>
        {
            { "count", 7 }
        };

        string result = ast.Render(context);

        Assert.AreEqual("Within Range", result);
    }

    [TestMethod]
    public void If_WithStringComparison_RendersCorrectly()
    {
        string template = "{% if name == 'John' %}Hello John{% else %}Hello Stranger{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();

        var context = new Dictionary<string, object>
        {
            { "name", "John" }
        };

        string result = ast.Render(context);

        Assert.AreEqual("Hello John", result);
    }

    [TestMethod]
    public void If_WithNestedIfElse_RendersCorrectly()
    {
        string template = "{% if isActive %}{% if isAdmin %}Admin{% else %}User{% endif %}{% else %}Inactive{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();

        var context = new Dictionary<string, object>
        {
            { "isActive", true },
            { "isAdmin", false }
        };

        string result = ast.Render(context);

        Assert.AreEqual("User", result);
    }

    [TestMethod]
    public void If_WithLogicalOr_RendersCorrectly()
    {
        // Arrange
        string template = "{% if isActive or isAdmin %}Access Granted{% else %}Access Denied{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();

        var context = new Dictionary<string, object>
        {
            { "isActive", false },
            { "isAdmin", true }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Access Granted", result);
    }

    [TestMethod]
    public void If_WithComplexNestedConditions_RendersCorrectly()
    {
        // Arrange
        string template = "{% if isActive %}{% if isAdmin and hasPermission %}Admin Access{% else %}Limited Access{% endif %}{% else %}No Access{% endif %}";
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(template);
        var parser = new Parser(tokens);
        var ast = parser.ParseTemplate();

        var context = new Dictionary<string, object>
        {
            { "isActive", true },
            { "isAdmin", true },
            { "hasPermission", false }
        };

        // Act
        string result = ast.Render(context);

        // Assert
        Assert.AreEqual("Limited Access", result);
    }
}

// Test model class for nested property tests
public class UserTestModel
{
    public bool IsActive { get; set; }
}