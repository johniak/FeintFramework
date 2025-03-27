using FeintFramework.Contrib.Admin.Forms;
using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Admin.Views;

public class AuthViews{

    public FeintHttpResponse Login(FeintHttpRequest request){
        if(request.Method == HttpMethods.Post){

        }
        var form = new LoginForm();
        return new FeintTemplateResponse("Admin/Templates/login.sbnhtml", new {form});
    }

}