using FeintFramework.Forms.Widgets;

namespace FeintFramework.Forms.Fields;

public abstract class BaseFormField
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string Label { get; set; } = "";
    public string Initial { get; set; } = "";
    public string HelpText { get; set; } = "";
    public bool Required { get; set; }
    public bool Disabled { get; set; }


    protected abstract BaseWidget widget { get; }

    public object toCSharpValue(string value)
    {
        return value;
    }
    public object prepareValue(object value)
    {
        return value;
    }
    public string Render()
    {
        return widget.Render();
    }
}