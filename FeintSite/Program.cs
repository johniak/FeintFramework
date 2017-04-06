using System;
using System.Collections.Generic;
using FeintApi.Serializers;
using FeintApi.Serializers.Fields;
using FeintSDK;
using FeintSDK.Middlewares;
using FeintServer.Core;
using Newtonsoft.Json;
namespace app
{

    class ExampleModel
    {
        public int TestId = 123; 
        public string Name = "abc";
    }
    

    class ExampleSerializer : Serializer<ExampleModel>
    {
        public ExampleSerializer(
          Dictionary<string, object> data = null,
          ExampleModel instance = null,
          Dictionary<string, object> context = null,
          bool many = false) : base(data, instance, context, many)
        {
        }
        public Field<int> TestId;
        public Field<string> Name;
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
             Console.WriteLine(new ExampleSerializer(instance: new ExampleModel()).Json);
            // Settings.Urls.Add(new Url(@"^/$", Program.Index));
            // Settings.Midelwares.Add(typeof(CookieSessionMiddleware));
            // Server s = new Server("0.0.0.0:5000");
            // s.Start();
        }
    }
}
