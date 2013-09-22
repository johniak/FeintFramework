using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using System.Reflection;
using DotLiquid;
using DotLiquid.FileSystems;
namespace Feint
{
    class Program
    {
        static void Main(string[] args)
        {
            Session s = new Session();
            Template.FileSystem = new LocalFileSystem(AppDomain.CurrentDomain.BaseDirectory + "FeintSite\\" + Settings.ViewsFolder.Replace("/", "\\"));
            List<Assembly> modulesAsseblies = new List<Assembly>();
            Assembly assembly = Assembly.LoadFrom(@"Site.dll");
            getMainMethod(assembly).Invoke(null, null);
            modulesAsseblies.Add(assembly);
            foreach (var str in Settings.Modules)
            {
                Assembly moduleAssembly = Assembly.LoadFrom(str + @".dll");

                getMainMethod(moduleAssembly).Invoke(null, null);
                modulesAsseblies.Add(moduleAssembly);
            }

            FeintORM.FeintORM orm = FeintORM.FeintORM.GetInstance(modulesAsseblies, Settings.databaseSettings);
            orm.CreateTablesFromModel();
            if (Settings.DebugMode)
            {
                DebugServer server = new DebugServer("http://*:8000/");
            }
            else
            {
                FastCGIServer server = new FastCGIServer("127.0.0.1:9000");
            }
        }
        public static MethodInfo getMainMethod(Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var t in types)
            {
                var method = t.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
                if (method != null)
                    return method;
            }
            return null;
        }
    }
}
