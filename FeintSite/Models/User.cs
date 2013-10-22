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

namespace Site.Models
{
    [UserModel]
    public class User : DBModel, ILiquidizable
    {
        [UserUsername(Label = "Username")]
        [DBProperty(Unique=true)]
        public String Username { get; set; }

        [UserPassword]
        [DBProperty(AllowNull=false)]
        public String Password { get; set; }
        
        
        [DBProperty(Unique=true)]
        public String Mail { get; set; }

        [DBProperty]
        public DateTime Created { get; set; }

        [DBProperty]
        public DateTime Updated { get; set; }

        public const   string LOGGED_IN_KEY = "isLogged";
        public  const string LOGGED_IN_USER_ID_KEY = "username";

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
			Project p = new Project () {
				Name = "Home",
				Owner = user
			};
			p.Save ();
            return 0;
        }
        public static bool IsLogged(Session session)
        {
            var logString = session.GetProperty(LOGGED_IN_KEY);
            if (logString == true.ToString())
                return true;
            return false;
        }
        public static User GetLoggedUser(Session session)
        {
            var logString = session.GetProperty(LOGGED_IN_KEY);
            if (logString == true.ToString())
            {
                var user = Ref<User>(int.Parse(session.GetProperty(LOGGED_IN_USER_ID_KEY)));
                if (user != null)
                    return null;
                else
                    return user;
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
        public override string ToString()
        {
            return Username;
        }
    }
}
