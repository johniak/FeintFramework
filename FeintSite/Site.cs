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
            Settings.Urls.Add(new Url(@"^/$", Views.Index));
            #region user administration
            Settings.Urls.Add(new Url(@"^/signin/$", Views.SignIn));
            Settings.Urls.Add(new Url(@"^/register/$", Views.Register));
            Settings.Urls.Add(new Url(@"^/signup/$", Views.SignUp));
            Settings.Urls.Add(new Url(@"^/logout/$", Views.LogOut));
            #endregion
            Settings.Urls.Add(new Url(@"^/messages/$", Views.Messages));
            Settings.Urls.Add(new Url(@"^/message/send/$", Views.SendMessage));
            Settings.Urls.Add(new Url(@"^/api/messages/$",Views.GetReciveBoxJson));
        }
    }
}
