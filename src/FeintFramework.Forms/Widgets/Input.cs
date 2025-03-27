

using FeintFramework.Forms.Widgets;
using Scriban;
using Scriban.Runtime;

namespace FeintFramework.Forms.Widgets;

public abstract class Input : BaseWidget
{
    public abstract string Type { get; }

    public override string TemplateFilePath => "Widgets/Templates/Input.sbnhtml";

    protected override TemplateContext Context
    {
        get
        {
            var context = base.Context;
            var obj = (ScriptObject) context.PopGlobal();
            obj.Add("type", Type);
            context.PushGlobal(obj);
            return context;
        }
    }
}

public class TextInput : Input
{
    public override string Type => "text";
}

