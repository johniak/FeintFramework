using static FeintFramework.Core.Shortcuts;

namespace FeintFramework.Forms.Widgets;

public abstract class BaseWidget
{
    public abstract string TemplateFilePath { get; }

    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    protected virtual Dictionary<string, object> Context
    {
        get
        {
            var context = new Dictionary<string, object>
            {
                { "widget", this },
                { "attributes", Attributes }
            };
            return context;
        }
    }

    public virtual string Render()
    {
        return RenderTemplate(TemplateFilePath, this.Context);
    }
}