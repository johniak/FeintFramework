using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using System.Reflection;
using Feint.FeintORM;
using DotLiquid;
using DotLiquid.FileSystems;
namespace Site
{
    public class Site
    {
        public static void Main()
        {
            Settings.databaseSettings = new DBSetting() {Helper= new PostgreSQLDatabaseHelper(),Host="127.0.0.1",Port=5432,User="postgres",Password="web4test",Name="task"};
            Settings.DebugMode = false;
            Settings.Urls.Add(new Url(@"^/$", Controlers.Application.Index));
            Settings.Urls.Add(new Url(@"^/dashboard/$", Controlers.Application.Dashboard));
            Settings.Urls.Add(new Url(@"^/dashboard/all/$", Controlers.Application.Dashboard));
            Settings.Urls.Add(new Url(@"^/dashboard/week/$", Controlers.Application.DashboardWeek));
            Settings.Urls.Add(new Url(@"^/dashboard/(?<project>[0-9]*?)/$", Controlers.Application.DashboardProject));
            Settings.Urls.Add(new Url(@"^/login/$", Controlers.Application.Login, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/login/$", Controlers.Application.Authenticate, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/register/$", Controlers.Application.Register, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/mobile/$", Controlers.Application.Mobile));
            Settings.Urls.Add(new Url(@"^/usage/$", Controlers.Application.Usage));
            Settings.Urls.Add(new Url(@"^/register/$", Controlers.Application.RegisterPost, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/logout/$", Controlers.Application.Mobile));
            Settings.Urls.Add(new Url(@"^/user/$", Controlers.Application.UpdateUser, RequestMethod.PUT));

            Settings.Urls.Add(new Url(@"^/tasks/$", Views.Index, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/$", Views.Index, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/$", Views.Index, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]*?)/tasks/$", Views.Index, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/$", Views.Index, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/$", Views.Index, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]*?)/tasks/$", Views.Index,RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/(?<task>[0-9]*?)/$", Views.Index,RequestMethod.PUT));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/(?<task>[0-9]*?)/$", Views.Index, RequestMethod.PUT));
            Settings.Urls.Add(new Url(@"^/projects/:project/tasks/(?<task>[0-9]*?)/$", Views.Index, RequestMethod.PUT));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/(?<task>[0-9]*?)/$", Views.Index, RequestMethod.DELETE));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/(?<task>[0-9]*?)/$", Views.Index, RequestMethod.DELETE));
            Settings.Urls.Add(new Url(@"^/projects/:project/tasks/(?<task>[0-9]*?)/$", Views.Index, RequestMethod.DELETE));


            Settings.Urls.Add(new Url(@"^/projects/$", Views.Index, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/$", Views.Index,RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]*?)/$", Views.Index, RequestMethod.DELETE));

            
        }
    }
}
