using FeintFramework.Contrib.Auth;
using FeintFramework.Http;
using static FeintFramework.Shortcuts;

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
        var appRecords = AdminHelpers.AppRecords;
        var context = new Dictionary<string, object>();
        context["appRecords"] = appRecords;
        return new FeintTemplateResponse("Admin/templates/home.html", context);
    }

    public static FeintHttpResponse AsView(FeintHttpRequest request)
    {
        var homepage = new Homepage(request);
        return homepage.getResponse();
    }
}