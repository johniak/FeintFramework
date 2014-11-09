using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feint.Core;
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
            ArgumentsEngine engine = new ArgumentsEngine();
            engine.Arguments.Add(new ConsoleArgument() {Analize=Program.RunServer,argCount=1,Pattern="^run$" });
            engine.Analize(args);
            Session s = new Session();
            Template.FileSystem = new LocalFileSystem(AppDomain.CurrentDomain.BaseDirectory + "FeintSite\\" + Settings.ViewsFolder.Replace("/", "\\"));
            List<Assembly> modulesAsseblies = new List<Assembly>();
            Assembly assembly = Assembly.LoadFrom(@"Site.dll");
            getMainMethod(assembly).Invoke(null, null);
            modulesAsseblies.Add(assembly);
            for (int i = 0; i < Settings.Modules.Count; i++)
            {
                var str = Settings.Modules[i];
                Assembly moduleAssembly = Assembly.LoadFrom(str + @".dll");
                getMainMethod(moduleAssembly).Invoke(null, null);
                modulesAsseblies.Add(moduleAssembly);
            }

            FeintORM.FeintORM orm = FeintORM.FeintORM.GetInstance(modulesAsseblies, Settings.databaseSettings, Settings.DebugMode);
            orm.CreateTablesFromModel();
            Server server;
            if (Settings.DebugMode)
            {
                server = new DebugServer(Settings.IpAddress);
            }
            else
            {
                server = new FastCgiServer(Settings.IpAddress);
            }
            server.Start();
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
        public static void RunServer(List<string> args)
        {
            Settings.IpAddress = args[0];
        }
    }
}
