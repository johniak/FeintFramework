using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    public class Settings
    {
        public static List<Url> Urls= new List<Url>();
        public static String IpAddress = "127.0.0.1:8000";
        public static String ViewsFolder = "views/";
        public static String StaticFolder = "static/";
        public static bool StaticCache = true;
        public static DBSetting databaseSettings = new DBSetting (){Helper=new SQLiteDatabaseHelper(),Name="db.sqlite"};
        public static bool DebugMode = true;
        public static List<string> Modules = new List<string>();
        public static Dictionary<String, object> Configurations = new Dictionary<string, object>();
    }
}
