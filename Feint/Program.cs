﻿using System;
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
            Assembly assembly = Assembly.LoadFrom(@"Site.dll");
            Template.FileSystem = new LocalFileSystem(AppDomain.CurrentDomain.BaseDirectory + "FeintSite\\" +Settings.ViewsFolder.Replace("/", "\\"));
            var site = assembly.GetType("Site.Site");
            site.GetMethod("Main").Invoke(null, null);
            FeintORM.FeintORM orm =  FeintORM.FeintORM.GetInstance(assembly, Settings.databaseSettings);
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
    }
}
