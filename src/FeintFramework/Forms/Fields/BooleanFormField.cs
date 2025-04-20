using FeintFramework.Forms.Widgets;

namespace FeintFramework.Forms.Fields;

public class BooleanFormField : BaseFormField
{
    protected override BaseWidget widget
    {
        get
        {
            var attrs = new Dictionary<string, string>() { { "name", this.Name },
            { "value", this.Initial},
            { "label", this.Label},
            { "help_text", this.HelpText},
            { "required", this.Required.ToString()},
            { "disabled", this.Disabled.ToString()}
            };
            var widget = new CheckboxInput();
            widget.Attributes =widget.Attributes.Concat(attrs).ToDictionary();
            return widget;
        }
    }

    public override object toCSharpValue(string? value)
    {
        if (value == null)
        {
            return false;
        }
        if (value == "on")
        {
            return true;
        }
        if (value == "off")
        {
            return false;
        }
        return value;
    }
    public override object? prepareValue(object? value)
    {
        return value;
    }
}