using System.Reflection;
using System.Threading.Tasks.Dataflow;
using FeintFramework.Core.Templating;
using FeintFramework.Core.Templating.Node;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace FeintFramework.Core.Http;

public class FeintTemplateResponse : FeintHttpResponse
{
    public FeintTemplateResponse(string templateFilePath) : base()
    {
        initialize(templateFilePath);
    }

    public FeintTemplateResponse(string templateFilePath, Dictionary<string, object> context)
    {
        initialize(templateFilePath, context);
    }

    public FeintTemplateResponse(string templateFilePath, object context)
    {
        var contextDictionary = context.GetType()
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .ToDictionary(prop => prop.Name, prop => prop.GetValue(context))!;
        initialize(templateFilePath, contextDictionary!);
    }

    protected void initialize(string templateFilePath, Dictionary<string, object>? context=null)
    {
        var template= File.ReadAllText(templateFilePath);
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(template);
        Parser parser = new Parser(tokens);
        TemplateNode ast = parser.ParseTemplate();
        if (context == null)
        {
            context = new Dictionary<string, object>();
        }
        Content = ast.Render(context);
        ContentType = "text/html";
    }
}