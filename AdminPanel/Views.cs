using DotLiquid;
using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feint.FeintORM;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json;
namespace AdminPanel
{
    class Views
    {
        static List<Type> modelsTypes;
        static Type userModelType;
        static PropertyInfo usernameProperty;
        static PropertyInfo passwordProperty;
        static string usernameLabel;
        static string passwordLabel;
        static List<string> modelNames = new List<string>();
        public static Response Login(Request request)
        {
            init();
            var response = new Response("admin/login.html", Hash.FromAnonymousObject(new { usernameName = "username", passwordName = "password", usernameLabel = (usernameLabel == null ? usernameProperty.Name : usernameLabel), passwordLabel = (passwordLabel == null ? passwordProperty.Name : passwordLabel) }));
            return response;
        }

        public static Response Auth(Request request)
        {
            init();
            var loginForm = Form.FromFormData<LogInForm>(request.FormData);
            if (loginForm.IsValid)
            {
                var usr = DBModel.Find(userModelType).Where().Eq(usernameProperty.Name, loginForm.username).And().Eq(passwordProperty.Name, MD5Hash(loginForm.password)).Execute();
                if (usr.Count == 1)
                {
                    request.Session.SetProperty(Site.SessionLoggedKey, usr[0].Id.ToString());
                    return Response.Redirect("/admin/dashboard/");
                }
            }
            return new Response("admin/login.html", Hash.FromAnonymousObject(new { usernameName = usernameProperty.Name, passwordName = passwordProperty.Name, usernameLabel = (usernameLabel == null ? usernameProperty.Name : usernameLabel), passwordLabel = (passwordLabel == null ? passwordProperty.Name : passwordLabel) })); ;
        }

        [AdminAuth]
        public static Response Dashboard(Request request)
        {
            init();
            var response = new Response("admin/dashboard.html", Hash.FromAnonymousObject(new { message = "Hello World!", models = modelNames }));
            return response;
        }

        [AdminAuth]
        public static Response Model(Request request)
        {
            init();
            var model = request.variables["model"].Value;
            Type t = modelsTypes[modelNames.IndexOf(model)];
            List<List<String>> table = new List<List<string>>();
            List<PropertyInfo> pis = Feint.FeintORM.FeintORM.GetInstance().getPropertiesFromClass(t).ToList();
            var lsh = new List<String>();
            foreach (var pi in pis)
            {
                lsh.Add(pi.Name);
            }
            var response = new Response("admin/model.html", Hash.FromAnonymousObject(new { message = "Hello World!", collumns = lsh, model = model }));
            return response;
        }


        [AdminAuth]
        public static Response ModelJson(Request request)
        {
            init();
            var model = request.variables["model"].Value;
            Type t = modelsTypes[modelNames.IndexOf(model)];
            List<DBModel> m = DBModel.Find(t).Where().Execute();
            List<List<String>> table = new List<List<string>>();
            List<PropertyInfo> pis = Feint.FeintORM.FeintORM.GetInstance().getPropertiesFromClass(t).ToList();
            var lsh = new List<String>();
            foreach (var pi in pis)
            {
                lsh.Add(pi.Name);
            }
            table.Add(lsh);
            foreach (var r in m)
            {
                var ls = new List<String>();
                foreach (var pi in pis)
                {
                    lsh.Add(pi.GetValue(r).ToString());
                }
                table.Add(ls);
            }
            return new Response(JsonConvert.SerializeObject(m));
        }


        static bool isUserModel(Type t)
        {
            var attrs = t.GetCustomAttributes(typeof(UserModel), true);
            if (attrs.Length > 0)
                return true;
            return false;
        }
        static PropertyInfo getUsernameProperty(Type t, out string label)
        {

            var pis = t.GetProperties();
            foreach (var pi in pis)
            {
                var attrs = pi.GetCustomAttributes(typeof(UserUsername), true);
                if (attrs.Length > 0)
                {
                    label = ((UserUsername)attrs[0]).Label;
                    return pi;
                }
            }
            label = null;
            return null;
        }
        static PropertyInfo getPasswordProperty(Type t, out string label)
        {

            var pis = t.GetProperties();
            foreach (var pi in pis)
            {
                var attrs = pi.GetCustomAttributes(typeof(UserPassword), true);
                if (attrs.Length > 0)
                {
                    label = ((UserPassword)attrs[0]).Label;
                    return pi;
                }
            }
            label = null;
            return null;
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

        static void init()
        {
            if (modelsTypes == null)
                modelsTypes = Feint.FeintORM.FeintORM.GetInstance().getAllModelClass().ToList();
            if (userModelType == null)
            {
                modelNames.Clear();
                foreach (var m in modelsTypes)
                {
                    modelNames.Add(m.Name);
                    if (isUserModel(m))
                    {
                        userModelType = m;
                        break;
                    }
                }
            }
            if (usernameProperty == null)
            {
                usernameProperty = getUsernameProperty(userModelType, out usernameLabel);
                passwordProperty = getPasswordProperty(userModelType, out passwordLabel);
            }
        }
    }
}
