using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace FeintFramework.Forms.Widgets;

public abstract class BaseWidget
{
    public abstract string TemplateFilePath { get; }

    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    protected virtual TemplateContext Context
    {
        get
        {
            var scriptObject = new ScriptObject();
            foreach (var item in this.Attributes)
            {
                scriptObject.Add(item.Key, item.Value); 
            }

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            return context;
        }
    }

    public virtual string Render()
    {
        var template = Template.Parse(File.ReadAllText(TemplateFilePath));
        return template.Render(Context);
    }
}