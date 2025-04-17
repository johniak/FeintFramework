using FeintFramework.Contrib.Auth;
using FeintFramework.Config;
using FeintFramework.Forms;
using FeintFramework.Forms.Fields;

namespace FeintFramework.Contrib.Admin.Forms;

public class LoginForm : Form
{
    public const string USER_KEY = "user";
    public CharFormField Username = new CharFormField() { Label = "Username", Disabled = false };
    public PasswordFormField Password = new PasswordFormField() { Label = "Password", Disabled = false };

    protected override void clean()
    {
        base.clean();
        if (!IsValid)
        {
            return;
        }

        var username = (string)CleanedData["Username"];
        var password = (string)CleanedData["Password"];
        var user = Configurator.Settings.AuthBackend().Authenticate(username, password);
        if (user == null)
        {
            AddError("Username and Password does not match"!);
            return;
        }
        CleanedData[USER_KEY] = user;
    }
}