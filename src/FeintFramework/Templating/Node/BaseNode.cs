namespace FeintFramework.Templating.Node;


public abstract class BaseNode
{
    public abstract string Render(Dictionary<string, object> context);
}