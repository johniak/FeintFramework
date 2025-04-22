using System.Diagnostics;
using FeintFramework.Contrib.Admin.Forms;
using FeintFramework.Contrib.Auth;
using FeintFramework.Config;
using FeintFramework.Http;
using static FeintFramework.Shortcuts;

namespace FeintFramework.Contrib.Admin.Views;

public class AuthViews
{

    public static FeintHttpResponse Login(FeintHttpRequest request)
    {
        if (request.User().IsAuthenticated){
            return Redirect("admin:home");
        }
        var form = new LoginForm();
        var watcher = Configurator.Settings.AdditionalSettings["HtmlWatcher"];
        Console.WriteLine(form.Errors);
        if (request.Method == HttpMethods.Post)
        {
            Dictionary<string, string> initial = request.Post;
            initial.Remove("Password");
            form.Data = request.Post;
            if (form.IsValid)
            {
                Configurator.Settings.AuthBackend().Login((IUser)form.CleanedData[LoginForm.USER_KEY], request);
                return Redirect("admin:home");
            }
            form.Initial = initial;


        }
        return new FeintTemplateResponse("Admin/Templates/login.html", new { form });
    }

}