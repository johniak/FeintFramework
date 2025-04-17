using FeintFramework.Forms.Widgets;

namespace FeintFramework.Forms.Fields;

public class PasswordFormField : CharFormField
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
            var widget = new PasswordInput();
            widget.Attributes =widget.Attributes.Concat(attrs).ToDictionary();
            return widget;
        }
    }

}