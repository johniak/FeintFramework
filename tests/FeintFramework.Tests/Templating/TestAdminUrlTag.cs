using FeintFramework.Config;
using FeintFramework.Config.Settings;
using FeintFramework.Db.Migrator;
using FeintFramework.Http;
using FeintFramework.Routing;
using FeintFramework.Templating;
using FeintFramework.Templating.Node;
using FeintFramework.Templating.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FeintFramework.Contrib.Admin.TemplateTags.Tests
{
    [TestClass]
    public class TestAdminUrlTag
    {
        public class TestSettings : BaseSettings
        {
            public override Type[] InstalledApps => new Type[] { };
            public override RootUrlPatterns RootUrlPatterns => new TestUrlPatterns();
            public override List<Type> Middlewares => new List<Type>();
            public override DatabaseHandler DatabaseHandler => throw new NotImplementedException();
            public override void ConfigureAdditionalSettings() { }
        }

        private static FeintHttpResponse view(FeintHttpRequest request)
        {
            return new FeintHttpResponse();
        }

        class TestUrlPatterns : RootUrlPatterns
        {
            public override List<UrlPattern> Urls => new List<UrlPattern>
            {
                new Routing.Path("/admin/blog/post/<int:pk>/change", view, "admin:blog:post:change"),
            };
        }

        [TestInitialize]
        public void Initialize()
        {
            Configurator.Settings = new TestSettings();
        }

        [TestMethod]
        public void AdminUrlTag_RendersCorrectUrl()
        {
            TagRegistry.Register("admin_url", AdminUrlTag.ParseAdminUrlTag);
            string template = "{% admin_url app model action pk %}";

            Lexer lexer = new Lexer();
            List<Token> tokens = lexer.Tokenize(template);
            Parser parser = new Parser(tokens);
            TemplateNode ast = parser.ParseTemplate();

            var context = new Dictionary<string, object>
            {
                { "app", "blog" },
                { "model", "post" },
                { "action", "change" },
                { "pk", 42 }
            };

            string output = ast.Render(context);
            Assert.AreEqual("/admin/blog/post/42/change", output);
        }
    }
}
