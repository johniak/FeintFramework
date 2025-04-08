using FeintFramework.Core.Http;
using FeintFramework.Core.Http.Exceptions;
using FeintFramework.Core.Routing;

namespace FeintFramework.Core;

public static class Shortcuts
{
    public static FeintHttpResponse Redirect(string urlName, Dictionary<string, object>? parameters=null)
    {
        var path = Router.Reverse(urlName);
        if (path == null)
        {
            throw new Http404($"Reverse not found for {urlName}");
        }
        return new FeintResponseRedirect(path);
    }
}