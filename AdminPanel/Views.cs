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
using System.Globalization;
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
            List<PropertyInfo> fis = Feint.FeintORM.FeintORM.GetInstance().getForeignersFromClass(t).ToList();
            var lsh = new List<String>();

            var p = pis[pis.Count - 1];
            pis.RemoveAt(pis.Count - 1);
            pis.Insert(0, p);
            List<String> forms = new List<string>();
            foreach (var pi in pis)
            {
                if (pi.Name != "Id")
                {
                    if (pi.PropertyType == typeof(string))
                    {
                        forms.Add("<tr><td><p><label style\"text-align:right;\">" + pi.Name + ": </label>" + "</td><td><input  type=\"text\" name=\"" + pi.Name + "\"/></td>" + "</p></tr>");
                    }
                    else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(long))
                    {
                        forms.Add("<tr><td><p><label>" + pi.Name + ": </label>" + "</td><td><input type=\"text\" name=\"" + pi.Name + "\"/></td>" + "</p></tr>");
                    } if (pi.PropertyType == typeof(DateTime))
                    {
                        forms.Add("<tr><td>" + pi.Name + ": </td><td><div class=\"input-append date datetime\"><input data-format=\"dd/MM/yyyy hh:mm:ss\" type=\"text\" name=\"" + pi.Name + "\"></input><span class=\"add-on\"><i data-time-icon=\"icon-time\" data-date-icon=\"icon-calendar\"></i></span></div></p></td></tr>");
                    }
                }
                lsh.Add(pi.Name);
            }


            foreach (var f in fis)
            {
                forms.Add("<tr><td><p><label>" + f.Name + " Id: </label>" + "</td><td><input type=\"text\" name=\"" + f.Name + "\"/></td>" + "</p></tr>");
                lsh.Add(f.Name);
            }
            var response = new Response("admin/model.html", Hash.FromAnonymousObject(new { message = "Hello World!", collumns = lsh, model = model, form = forms }));
            return response;
        }
        [AdminAuth]
        public static Response DeleteModel(Request request)
        {
            init();
            try
            {
                var model = request.variables["model"].Value;
                Type t = modelsTypes[modelNames.IndexOf(model)];
                var dbm = DBModel.Ref(long.Parse(request.variables["id"].Value), t); // DBModel.Find(t).Where().Eq("Id", request.variables["id"].Value).Execute()[0];
                dbm.Remove();
                return new Response(JsonConvert.SerializeObject(true));
            }
            catch (Exception ex)
            {
                return new Response(JsonConvert.SerializeObject(false));
            }
        }

        [AdminAuth]
        public static Response AddModel(Request request)
        {
            try
            {
                var model = request.variables["model"].Value;
                Type t = modelsTypes[modelNames.IndexOf(model)];
                List<PropertyInfo> pis = Feint.FeintORM.FeintORM.GetInstance().getPropertiesFromClass(t).ToList();
                List<PropertyInfo> fis = Feint.FeintORM.FeintORM.GetInstance().getForeignersFromClass(t).ToList();
                DBModel obj = (DBModel)Activator.CreateInstance(t);
                foreach (var p in pis)
                {
                    if (p.Name == "Id")
                        continue;
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(obj, request.FormData[p.Name]);
                    }
                    else if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(obj, int.Parse(request.FormData[p.Name]));
                    }
                    else if (p.PropertyType == typeof(long))
                    {
                        p.SetValue(obj, long.Parse(request.FormData[p.Name]));
                    }
                    else if (p.PropertyType == typeof(float))
                    {
                        p.SetValue(obj, float.Parse(request.FormData[p.Name]));
                    }
                    else if (p.PropertyType == typeof(double))
                    {
                        p.SetValue(obj, double.Parse(request.FormData[p.Name]));
                    }
                    else if (p.PropertyType == typeof(DateTime))
                    {
                        p.SetValue(obj, DateTime.ParseExact(request.FormData[p.Name], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
                foreach (var f in fis)
                {
                    int id = int.Parse(request.FormData[f.Name]);
                    var foreginType = typeof(DBForeignKey<>).MakeGenericType(f.PropertyType.GetGenericArguments()[0]);
                    dynamic fobj = Activator.CreateInstance(foreginType, true);
                    PropertyInfo pi = foreginType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                    pi.SetValue(fobj, id);
                    f.SetValue(obj, fobj);
                }
                obj.Save();
                return new Response(JsonConvert.SerializeObject(obj));
            }
            catch (Exception ex)
            {
                return new Response(JsonConvert.SerializeObject(false));
            }
        }

        [AdminAuth]
        public static Response ModelJson(Request request)
        {
            init();

            var model = request.variables["model"].Value;
            var startIndex = long.Parse(request.variables["startIndex"].Value);
            var count = long.Parse(request.variables["count"].Value);
            var collumn = request.variables["collumn"].Value;
            var asc = bool.Parse(request.variables["asc"].Value);
            string search = request.variables["search"].Value;
            Type t = modelsTypes[modelNames.IndexOf(model)];
            var where = DBModel.Find(t).Where();
            List<PropertyInfo> pis = Feint.FeintORM.FeintORM.GetInstance().getPropertiesFromClass(t).ToList();
            int i = 0;
            foreach (var pi in pis)
            {
                if (pi.PropertyType == typeof(string))
                {

                    where = where.Like(pi.Name, "%" + search + "%");
                    if (isLikablePropertyAfter(pis, i))
                        where = where.Or();
                }
                i++;
            }

            List<DBModel> data = where.Limit(startIndex, count).OrderBy(collumn, asc).Execute();
            List<Dictionary<string, string>> listOfDicts = new List<Dictionary<string, string>>();
            foreach (var dbm in data)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var pi in pis)
                {
                    if (pi.PropertyType == typeof(DateTime))
                    {

                        dict.Add(pi.Name, ((DateTime)pi.GetValue(dbm)).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        dict.Add(pi.Name, (pi.GetValue(dbm)).ToString());
                    }
                }
                listOfDicts.Add(dict);
            }
            return new Response(JsonConvert.SerializeObject(listOfDicts));
        }
        [AdminAuth]
        public static Response ModelRow(Request request)
        {
            try
            {
                var model = request.variables["model"].Value;
                Type t = modelsTypes[modelNames.IndexOf(model)];
                var dbm = DBModel.Ref(long.Parse(request.variables["id"].Value), t);
                
                Dictionary<string, string> dict = new Dictionary<string, string>();
                List<PropertyInfo> pis = Feint.FeintORM.FeintORM.GetInstance().getPropertiesFromClass(t).ToList();
                foreach (var pi in pis)
                {
                    if (pi.PropertyType == typeof(DateTime))
                    {

                        dict.Add(pi.Name, ((DateTime)pi.GetValue(dbm)).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        dict.Add(pi.Name, (pi.GetValue(dbm)).ToString());
                    }
                }
                return new Response(JsonConvert.SerializeObject(dict));
            }
            catch (Exception ex)
            {
                return new Response(JsonConvert.SerializeObject(false));
            }
        }
        static bool isLikablePropertyAfter(List<PropertyInfo> lsh, int index)
        {

            for (index++; index < lsh.Count; index++)
            {
                if (lsh[index].PropertyType == typeof(string))
                {
                    return true;
                }
            }
            return false;
        }

        [AdminAuth]
        public static Response ModelJsonCount(Request request)
        {
            init();
            var model = request.variables["model"].Value;
            Type t = modelsTypes[modelNames.IndexOf(model)];
            var m = DBModel.Find(t).Where().Count();
            return new Response(JsonConvert.SerializeObject(m));
        }

        [AdminAuth]
        public static Response EditRow(Request request)
        {
            init();
            //   try
            //   {
            var model = request.variables["model"].Value;
            var id = long.Parse(request.variables["id"].Value);
            Type t = modelsTypes[modelNames.IndexOf(model)];


            List<PropertyInfo> pis = Feint.FeintORM.FeintORM.GetInstance().getPropertiesFromClass(t).ToList();
            List<PropertyInfo> fis = Feint.FeintORM.FeintORM.GetInstance().getForeignersFromClass(t).ToList();
            DBModel obj = DBModel.Ref(id, t);
            foreach (var p in pis)
            {
                if (p.Name == "Id")
                    continue;
                if (p.PropertyType == typeof(string))
                {
                    p.SetValue(obj, request.FormData[p.Name]);
                }
                else if (p.PropertyType == typeof(int))
                {
                    p.SetValue(obj, int.Parse(request.FormData[p.Name]));
                }
                else if (p.PropertyType == typeof(long))
                {
                    p.SetValue(obj, long.Parse(request.FormData[p.Name]));
                }
                else if (p.PropertyType == typeof(float))
                {
                    p.SetValue(obj, float.Parse(request.FormData[p.Name]));
                }
                else if (p.PropertyType == typeof(double))
                {
                    p.SetValue(obj, double.Parse(request.FormData[p.Name]));
                }
                else if (p.PropertyType == typeof(DateTime))
                {
                    p.SetValue(obj, DateTime.ParseExact(request.FormData[p.Name], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            foreach (var f in fis)
            {
                int idp = int.Parse(request.FormData[f.Name]);
                var foreginType = typeof(DBForeignKey<>).MakeGenericType(f.PropertyType.GetGenericArguments()[0]);
                dynamic fobj = Activator.CreateInstance(foreginType, true);
                PropertyInfo pi = foreginType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                pi.SetValue(fobj, idp);
                f.SetValue(obj, fobj);
            }
            obj.Save();
            return new Response(JsonConvert.SerializeObject(obj));
            //}
            //catch (Exception ex)
            //{
            //    return new Response(JsonConvert.SerializeObject(false));
            //}
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
