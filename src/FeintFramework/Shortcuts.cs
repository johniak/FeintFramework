using System.Reflection;
using FeintFramework.Http;
using FeintFramework.Http.Exceptions;
using FeintFramework.Routing;
using FeintFramework.Templating;

namespace FeintFramework;

public static class Shortcuts
{
    public static string? ReverseUrl(string urlName, Dictionary<string, object>? parameters = null)
    {
        return Router.Reverse(urlName, parameters);
    }
    public static FeintHttpResponse Redirect(string urlName, Dictionary<string, object>? parameters = null)
    {
        var path = Router.Reverse(urlName, parameters);
        if (path == null)
        {
            throw new Http404($"Reverse not found for {urlName}");
        }
        return new FeintResponseRedirect(path);
    }

    public static string RenderTemplate(string templateFilePath, Dictionary<string, object>? context = null)
    {
        var ast = TemplateLoader.Load(templateFilePath);
        if (context == null)
        {
            context = new Dictionary<string, object>();
        }
        return ast.Render(context);
    }
    public static string RenderTemplate(string templateFilePath, object context)
    {
        var contextDictionary = context.GetType()
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .ToDictionary(prop => prop.Name, prop => prop.GetValue(context))!;
        return RenderTemplate(templateFilePath, contextDictionary!);
    }
}