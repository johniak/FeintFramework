using FeintFramework.Forms;
using FeintFramework.Forms.Fields;

namespace FeintFramework.Contrib.Admin.Forms;

public class LoginForm : Form
{
    public CharFormField Username = new CharFormField() { Label = "Username", Disabled = false };
    public PasswordFormField Password = new PasswordFormField() { Label = "Password", Disabled = false };
}