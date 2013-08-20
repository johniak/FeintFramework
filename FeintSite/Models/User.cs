using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using Feint.FeintORM;
using DotLiquid;
using Site.Models;
using System.Security.Cryptography;

namespace Site
{
    class User : DBModel, ILiquidizable
    {
        [DBProperty(false, false, true, false)]
        public String Username { get; set; }

        [DBProperty(false, false, false, false)]
        public String Password { get; set; }

        [DBProperty(false, false, true, false)]
        public String Mail { get; set; }

        [DBProperty]
        public DateTime Created { get; set; }

        [DBProperty]
        public DateTime Updated { get; set; }


        public static bool SignIn(String username, String password)
        {
            var users = User.Find<User>().Where().Eq("Username", username).Execute();
            if (users.Count <= 0)
                return false;
            password = MD5Hash(password);
            if (users[0].Password.Length != 0 && users[0].Password == password)
                return true;
            return false;
        }
        public static int SignUp(String username, String password, String mail)
        {
            if (User.Find<User>().Where().Eq("Username", username).Execute().Count > 0)
                return 1;
            if (password.Length < 4)
                return 2;
            if (mail.Length < 5)
                return 3;
            User user = new User();
            user.Username = username;
            user.Password = MD5Hash(password);
            user.Mail = mail;
            user.Created = DateTime.Now;
            user.Updated = DateTime.Now;
            user.Save();
            return 0;
        }
        public static bool IsLogged(Session session)
        {
            var logString = session.GetProperty("isLogged");
            if (logString == true.ToString())
                return true;
            return false;
        }
        public static User GetLoggedUser(Session session)
        {
            var logString = session.GetProperty("isLogged");
            if (logString == true.ToString())
            {
                var users = Find<User>().Where().Eq("Username", session.GetProperty("username")).Execute();
                if (users.Count != 1)
                    return null;
                else
                    return users[0];
            }
            else
            {
                return null;
            }
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public object ToLiquid()
        {
            return new { Id, Username, Password, };
        }
    }
}
