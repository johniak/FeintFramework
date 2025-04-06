using FeintFramework.Core.Config;
using FeintFramework.Core.Http;
using FeintFramework.Core.Routing;

namespace FeintFramework.Core.Views;


public static class DefaultViews
{

    public static FeintHttpResponse NotFound(FeintHttpRequest request, Exception e)
    {
        var patterns = Router.BuildFullUrlPatternList(Configurator.Settings.RootUrlPatterns.Urls);
        var urlconf = Configurator.Settings.RootUrlPatterns.GetType().FullName;
        return new FeintTemplateResponse("Views/Templates/technical_404.sbnhtml", new { request, patterns, urlconf });
    }
}