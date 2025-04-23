using FeintFramework.Forms.Widgets;

namespace FeintFramework.Forms.Fields;

public class DateTimeFormField : CharFormField
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
            var widget = new TextInput();
            widget.Attributes = widget.Attributes.Concat(attrs).ToDictionary();
            return widget;
        }
    }

    public override object toCSharpValue(string? value)
    {
        if (value == null)
        {
            return null;
        }
        if (value == "")
        {
            return null;
        }
        try
        {
            return DateTime.Parse(value);
        }
        catch (FormatException)
        {
            throw new ValidationException("Invalid date format");
        }
    }

}