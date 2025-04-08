namespace FeintFramework.Core.Templating.Node;


public abstract class BaseNode
{
    public abstract string Render(Dictionary<string, object> context);
}