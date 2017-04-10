﻿using System;
using System.Collections.Generic;
using FeintApi.Serializers;
using FeintApi.Serializers.Fields;
using FeintSDK;
using FeintSDK.Middlewares;
using FeintServer.Core;
using Newtonsoft.Json;
using System.Reflection;
namespace FeintSite
{

    class ExampleModelSerializer : ModelSerializer<ExampleModel>
    {
        public ExampleModelSerializer(object instance = null, object data = null, bool many = false) : base(instance, data, many)
        {
            ModelFields = new string[] { "ExamplePropertyString", "ExamplePropertyInteger" };
        }
    }

    class Program
    {
        public static Response Index(Request request)
        {
            var response = new Response("Hi! Im your dashboard");
            return response;
        }

        static void Main(string[] args)
        {
            // var inst = new ExampleModel();
            // Console.WriteLine(new ExampleSerializer(instance: inst).Json);
            // var instArr = new[] { inst, inst };
            //Console.WriteLine(new ExampleSerializer(instance: instArr, many: true).Json);
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"ExamplePropertyString", "abc"},
                {"ExamplePropertyInteger", 534}
            };
            var exms = new ExampleModelSerializer(data: data);
            var instance = exms.Save();
            exms = new ExampleModelSerializer(instance: instance);
            Console.WriteLine(exms.Json);
            // var ex2 = new Example2Model();
            // var ex2ms = new Example2ModelSerializer(instance: ex2);
            // Console.WriteLine(ex2ms.Json);
            // Settings.Urls.Add(new Url(@"^/$", Program.Index));
            // Settings.Midelwares.Add(typeof(CookieSessionMiddleware));
            // Server s = new Server("0.0.0.0:5000");
            // s.Start();
        }
    }
}
