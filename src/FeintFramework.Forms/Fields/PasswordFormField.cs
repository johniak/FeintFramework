using FeintFramework.Forms.Widgets;

namespace FeintFramework.Forms.Fields;

public class PasswordFormField : BaseFormField
{
    protected override BaseWidget widget => new PasswordInput()
    {
        Attributes = new Dictionary<string, string>() { { "name", this.Name },
    { "value", this.Initial},
    { "label", this.Label},
    { "help_text", this.HelpText},
    { "required", this.Required.ToString()},
    { "disabled", this.Disabled.ToString()}}
    };

}