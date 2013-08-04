using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using Feint.FeintORM;
using DotLiquid;
using Site.Models;

namespace Site
{
    class User:DBModel,ILiquidizable
    {
        [DBProperty(false,false,true,false)]
        public String Username { get; set; }

        [DBProperty(false,false,false,false)]
        public String Password { get; set; }

        [DBForeignKey]
        public SampleModel model { get; set; }

        public static bool SignIn(String username, String password)
        {
            User user= User.getOne<User>(u => u.Username == username);
            if (user == null)
                return false;
            if (user.Password.Length != 0 && user.Password == password)
                return true;
            return false;
        }
        public static bool SignUp(String username,String password)
        {
            User user = User.getOne<User>(u => u.Username == username);
            if (user != null)
                return false;
            if (password.Length == 0)
                return false;
            user = new User();
            user.Username = username;
            user.Password = password;
            user.Save();
            return true;
        }
        public static bool isLogged(Session session)
        {
            var logString = session.GetProperty("isLogged");
            if (logString == true.ToString())
                return true;
            return false;
        }
        public object ToLiquid()
        {
            return new {Id, Username, Password,};
        }
    }
}
