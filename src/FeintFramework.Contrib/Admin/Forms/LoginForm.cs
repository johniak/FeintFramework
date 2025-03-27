using FeintFramework.Forms;
using FeintFramework.Forms.Fields;

namespace FeintFramework.Contrib.Admin.Forms;

class LoginForm : Form
{
    public CharFormField Username = new CharFormField() { Label = "Username" };
    public CharFormField Password = new CharFormField() { Label = "Password" };
}