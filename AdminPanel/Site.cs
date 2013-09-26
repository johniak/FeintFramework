using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel
{
    class Site
    {
        public const string AdminLoginUrl = "/admin/login/";
        public const string SessionLoggedKey = "adminlogged"; 
        public static void Main()
        {
            Settings.Urls.Add(new Url("^/admin/login/$", Views.Login, RequestMethod.GET));
            Settings.Urls.Add(new Url("^/admin/login/$", Views.Auth, RequestMethod.POST));
            Settings.Urls.Add(new Url("^/admin/dashboard/$", Views.Dashboard));
            Settings.Urls.Add(new Url("^/admin/$", Views.Dashboard));
            Settings.Urls.Add(new Url("^/admin/model/(?<model>.*?)/json/$", Views.ModelJson, RequestMethod.GET));
            Settings.Urls.Add(new Url("^/admin/model/(?<model>.*?)/$", Views.Model, RequestMethod.GET));
        }
    }
}
