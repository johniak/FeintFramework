

using System.Collections;
using System.Text;
using FeintFramework.Core.Templating.Node;
namespace FeintFramework.Core.Templating.Tags;

public class IncludeNode : BaseNode
{
    public string TemplateName { get; }

    public IncludeNode(string templateName)
    {
        TemplateName = templateName;
    }

    public override string Render(Dictionary<string, object> context)
    {
        TemplateNode includedTemplate = TemplateLoader.Load(TemplateName);
        return includedTemplate.Render(context);
    }
}


public static class IncludeTag
{
    public static BaseNode ParseIncludeTag(Parser parser, Token token)
    {
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || parts[0].ToLowerInvariant() != "include")
        {
            throw new Exception("Invalid syntax for include tag. Expected {% include \"template.html\" %}");
        }
        string templateName = parts[1].Trim();
        if ((templateName.StartsWith("\"") && templateName.EndsWith("\"")) ||
            (templateName.StartsWith("'") && templateName.EndsWith("'")))
        {
            templateName = templateName.Substring(1, templateName.Length - 2);
        }
        return new IncludeNode(templateName);
    }
}