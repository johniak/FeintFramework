using System.Diagnostics;
using FeintFramework.Contrib.Admin.Forms;
using FeintFramework.Contrib.Auth;
using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Admin.Views;

public class AuthViews
{

    public static FeintHttpResponse Login(FeintHttpRequest request)
    {
        var form = new LoginForm();
        if (request.Method == HttpMethods.Post)
        {
            Dictionary<string, string> initial = request.Post;
            initial.Remove("Password");
            form.Data = request.Post;
            form.Initial = initial;

            var username = (string)form.CleanedData["Username"];
            var password = (string)form.CleanedData["Password"];
            Console.WriteLine("Poscik");
            Console.WriteLine(username);
            Console.WriteLine(password);
            try
            {
                var user = User.Objects.First(o => o.Username == username);
                user.VerifyPassword(password);
            }catch(Exception)
            {
                
            }
        }
        return new FeintTemplateResponse("Admin/Templates/login.sbnhtml", new { form });
    }

}