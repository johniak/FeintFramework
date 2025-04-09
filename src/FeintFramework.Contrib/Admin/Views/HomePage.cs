using FeintFramework.Contrib.Auth;
using FeintFramework.Core.Http;
using static FeintFramework.Core.Shortcuts;

namespace FeintFramework.Contrib.Admin.Views;

public class Homepage
{
    protected FeintHttpRequest request { get; set; }
    protected Homepage(FeintHttpRequest request)
    {
        this.request = request;
    }

    protected FeintHttpResponse getResponse()
    {
        if (request.User() == null)
            return Redirect("admin:login");
        return new FeintTemplateResponse("Admin/templates/home.html");
    }

    public static FeintHttpResponse AsView(FeintHttpRequest request)
    {
        var homepage = new Homepage(request);
        return homepage.getResponse();
    }
}