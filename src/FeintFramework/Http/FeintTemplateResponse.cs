using System.Reflection;
using static FeintFramework.Shortcuts;

namespace FeintFramework.Http;

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
        Content = RenderTemplate(templateFilePath, context);
        ContentType = "text/html";
    }
}