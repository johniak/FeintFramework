using FeintFramework.Routing;
using FeintFramework.Config;
using FeintFramework.Contrib.Admin.Views;

namespace FeintFramework.Contrib.Admin;

public class AdminUrls : UrlPatterns
{
    public static List<UrlPattern> PrefixedUrls = new List<UrlPattern>(){
        new UrlPattern("/login$", AuthViews.Login, "login"),
        new UrlPattern("/$", Homepage.AsView, "home"),
    };

    public override List<UrlPattern> Urls
    {
        get
        {
            return new List<UrlPattern>
            {
                new UrlPattern($"/{Configurator.Settings.AdminUrlPrefix()}",PrefixedUrls, "admin")
            };
        }
    }
}