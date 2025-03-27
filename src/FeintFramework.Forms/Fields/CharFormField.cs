using FeintFramework.Forms.Widgets;

namespace FeintFramework.Forms.Fields;

public class CharFormField : BaseFormField
{
    protected override BaseWidget widget => new TextInput()
    {
        Attributes = new Dictionary<string, string>() { { "name", this.Name },
    { "value", this.Initial},
    { "label", this.Label},
    { "help_text", this.HelpText},
    { "required", this.Required.ToString()},
    { "disabled", this.Disabled.ToString()}}
    };

}