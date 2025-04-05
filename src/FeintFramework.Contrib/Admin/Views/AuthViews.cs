using System.Diagnostics;
using FeintFramework.Contrib.Admin.Forms;
using FeintFramework.Contrib.Auth;
using FeintFramework.Core.Config;
using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Admin.Views;

public class AuthViews
{

    public static FeintHttpResponse Login(FeintHttpRequest request)
    {
        var form = new LoginForm();
        Console.WriteLine(form.Errors);
        if (request.Method == HttpMethods.Post)
        {
            Dictionary<string, string> initial = request.Post;
            initial.Remove("Password");
            form.Data = request.Post;
            if (form.IsValid)
            {
                Configurator.Settings.AuthBackend().Login((IUser)form.CleanedData[LoginForm.USER_KEY], request);
                return new FeintTemplateResponse("Admin/Templates/tmp.sbnhtml", new {  });
            }
            form.Initial = initial;


        }
        return new FeintTemplateResponse("Admin/Templates/login.sbnhtml", new { form });
    }

}