using System.Diagnostics;
using FeintFramework.Contrib.Admin.Forms;
using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Admin.Views;

public class AuthViews
{

    public static FeintHttpResponse Login(FeintHttpRequest request)
    {

        if (request.Method == HttpMethods.Post)
        {
            Console.WriteLine("Poscik");
            foreach( var item in request.Post){
                Console.WriteLine(item.Key);
                Console.WriteLine(item.Value);
            }
        }
        var form = new LoginForm();
        return new FeintTemplateResponse("Admin/Templates/login.sbnhtml", new { form });
    }

}