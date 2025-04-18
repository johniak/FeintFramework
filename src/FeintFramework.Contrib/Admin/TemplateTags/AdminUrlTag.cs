

using System.Collections;
using System.Text;
using FeintFramework.Templating;
using FeintFramework.Templating.Node;
using static FeintFramework.Shortcuts;
namespace FeintFramework.Contrib.Admin.TemplateTags;

public class AdminUrlNode : BaseNode{
    public string AppName { get; }
    public string ModelName { get; }
    public string Action { get; }

    public string Pk { get; }

    public AdminUrlNode(string appName, string modelName, string action, string pk)
    {
        AppName = appName;
        ModelName = modelName;
        Action = action;
        Pk = pk;
    }

    public override string Render(Dictionary<string, object> context)
    {
        var appNameValue = VariableNode.GetVariableValue(AppName, context);
        var modelNameValue = VariableNode.GetVariableValue(ModelName, context);
        var actionValue = VariableNode.GetVariableValue(Action, context);
        var pkValue = VariableNode.GetVariableValue(Pk, context);
        var urlName =  $"admin:{appNameValue}:{modelNameValue}:{actionValue}";
        var keywordArgs = new Dictionary<string, object>
        {
            { "pk", pkValue }
        };
        return ReverseUrl(urlName, keywordArgs.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value))!;
    }
}




public static class AdminUrlTag
{
    public static BaseNode ParseAdminUrlTag(Parser parser, Token token)
    {
        var parts = token.Content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length !=5 || !parts[0].Equals("admin_url", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Invalid url tag syntax. Expected: {% admin_url appNameVariable modelNameVariable actionVariable pkVariable %}");
        }
        
        
        return new AdminUrlNode(parts[1], parts[2], parts[3], parts[4]);
    }
}
