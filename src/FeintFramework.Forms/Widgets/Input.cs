


namespace FeintFramework.Forms.Widgets;

public abstract class Input : BaseWidget
{
    public abstract string Type { get; }

    public override string TemplateFilePath => "Widgets/Templates/Input.html";

    public Input(): base()
    {
        Attributes["type"] = Type;
    }
}

public class TextInput : Input
{
    public TextInput(): base()
    {
    }
    public override string Type => "text";
}


public class PasswordInput : Input
{
    public PasswordInput(): base()
    {
    }
    public override string Type => "password";
}

