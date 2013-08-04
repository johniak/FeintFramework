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
        public static String ViewsFolder = "views/";
        public static String StaticFolder = "static/";
        public static DBSetting databaseSettings = new DBSetting (){Helper=new SQLiteDatabaseHelper(),Name="db.sqlite"};

    }
}
