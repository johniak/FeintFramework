using FeintFramework.Config;
using FeintFramework.Config.Settings;
using FeintFramework.Http;
using FeintFramework.Routing;
using FeintFramework.Templating.Node;
using FeintFramework.Templating.Tags;
using FeintFramework.Db.Migrator;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework.Utilities;

namespace FeintFramework.Templating;

[TestClass]
public sealed class TestParserUrlTag
{
      public class TestSettings : BaseSettings
    {
        public override Type[] InstalledApps => [
        ];


        public override RootUrlPatterns RootUrlPatterns => new TestUrlPatterns();

        public override List<Type> Middlewares => new List<Type>
        {
        };

        public override DatabaseHandler DatabaseHandler => throw new NotImplementedException();

        public override void ConfigureAdditionalSettings()
        {
            
        }
    }
    private static FeintHttpResponse view(FeintHttpRequest request)
    {
        return new FeintHttpResponse();
    }

    class TestUrlPatterns : RootUrlPatterns
    {
        public override List<UrlPattern> Urls
        {
            get
            {
                return new List<UrlPattern>
            {
                new UrlPattern("/example",view, "example"),

                new FeintFramework.Routing.Path("/parametrized/<int:pk>",view, "parametrized"),
            };
            }
        }
    }

    [TestInitialize]
    public void Initialize()
    {
        Configurator.Settings = new TestSettings();
    }

    

    private string RenderTemplate(string template, Dictionary<string, object> context)
    {
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();
        return ast.Render(context);
    }

    [TestMethod]
    public void UrlTag_Render_GeneratesCorrectUrl()
    {
        TagRegistry.Register("url", UrlTag.ParseUrlTag);
        string template = "Generated URL: {% url 'parametrized' pk='value1' %}";

        Dictionary<string, object> context = new Dictionary<string, object>();

        string output = RenderTemplate(template, context);
        Assert.AreEqual("Generated URL: /parametrized/value1", output);
    }

    public void UrlTag_Render_GeneratesCorrectUrlWithIntPk()
    {
        TagRegistry.Register("url", UrlTag.ParseUrlTag);
        string template = "Generated URL: {% url 'parametrized' pk=1327 %}";

        Dictionary<string, object> context = new Dictionary<string, object>();

        string output = RenderTemplate(template, context);
        Assert.AreEqual("Generated URL: /parametrized/1327", output);
    }

    [TestMethod]
    public void UrlTag_Render_WithoutArguments_GeneratesCorrectUrl()
    {
        TagRegistry.Register("url", UrlTag.ParseUrlTag);
        string template = "Generated URL: {% url 'example' %}";

        Dictionary<string, object> context = new Dictionary<string, object>();

        string output = RenderTemplate(template, context);
        Assert.AreEqual("Generated URL: /example", output);
    }

    // [TestMethod]
    // public void UrlTag_Render_WithPositionalArguments_GeneratesCorrectUrl()
    // {
    //     TagRegistry.Register("url", UrlTag.ParseUrlTag);
    //     string template = "Generated URL: {% url 'positional_view' 'arg1' 'arg2' %}";

    //     Dictionary<string, object> context = new Dictionary<string, object>();

    //     string output = RenderTemplate(template, context);
    //     Assert.AreEqual("Generated URL: /positional_view/arg1/arg2", output);
    // }
}