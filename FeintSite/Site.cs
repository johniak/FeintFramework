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
using Site.Models;


namespace Site
{
    class Site
    {
        public static void Main()
        {

            Settings.databaseSettings = new DBSetting() {Helper= new PostgreSQLDatabaseHelper(),Host="127.0.0.1",Port=5432,User="postgres",Password="test",Name="task"};

            //Settings.databaseSettings = new DBSetting() { Helper = new SQLiteDatabaseHelper(),Name="db.sqlite"};
           
            
            Settings.DebugMode = false;
            Settings.StaticCache = true;
            Settings.Modules.Add("AdminPanel");
            Settings.Modules.Add("Api");

            Settings.Urls.Add(new Url(@"^/$", Controlers.Application.Index));
            Settings.Urls.Add(new Url(@"^/dashboard/$", Controlers.Application.Dashboard));
            Settings.Urls.Add(new Url(@"^/dashboard/all/$", Controlers.Application.Dashboard));
            Settings.Urls.Add(new Url(@"^/dashboard/week/$", Controlers.Application.DashboardWeek));
            Settings.Urls.Add(new Url(@"^/dashboard/(?<project>[0-9]+?)/$", Controlers.Application.DashboardProject));
            Settings.Urls.Add(new Url(@"^/login/$", Controlers.Application.Login, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/login/$", Controlers.Application.Authenticate, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/register/$", Controlers.Application.Register, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/mobile/$", Controlers.Application.Mobile));
            Settings.Urls.Add(new Url(@"^/usage/$", Controlers.Application.Usage));
            Settings.Urls.Add(new Url(@"^/register/$", Controlers.Application.RegisterPost, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/logout/$", Controlers.Application.Logout));
            Settings.Urls.Add(new Url(@"^/user/$", Controlers.Application.UpdateUser, RequestMethod.PUT));
        }
    }
}
