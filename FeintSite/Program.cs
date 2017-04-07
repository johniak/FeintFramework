using System;
using System.Collections.Generic;
using FeintApi.Serializers;
using FeintApi.Serializers.Fields;
using FeintSDK;
using FeintSDK.Middlewares;
using FeintServer.Core;
using Newtonsoft.Json;
using System.Reflection;
namespace app
{
    class Example2Model
    {
        public int SampleId = 666;
        public int SampleId2 = 666;
    }
    class ExampleModel
    {
        public int TestId = 123;
        public string Name = "abc";
        public Example2Model ForeignKey = new Example2Model();
        //public List<Example2Model> ForeignKey = new List<Example2Model>(){ new Example2Model(),new Example2Model()};
    }
    class Example2Serializer : Serializer<ExampleModel>
    {
        public Example2Serializer(object instance = null, object data = null, bool many = false) : base(instance, data, many)
        {

        }
        public Example2Serializer()
        {

        }
        public Field<int> SampleId;
    }

    class ExampleSerializer : Serializer<ExampleModel>
    {
        public ExampleSerializer(object instance = null, object data = null, bool many = false) : base(instance, data, many)
        {

        }

        public Field<int> TestId;
        public Field<string> Name;
        public Example2Serializer ForeignKey;
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
            var inst = new ExampleModel();
            Console.WriteLine(new ExampleSerializer(instance: inst).Json);
            var instArr = new[] { inst, inst };
            //Console.WriteLine(new ExampleSerializer(instance: instArr, many: true).Json);

            // Settings.Urls.Add(new Url(@"^/$", Program.Index));
            // Settings.Midelwares.Add(typeof(CookieSessionMiddleware));
            // Server s = new Server("0.0.0.0:5000");
            // s.Start();
        }
    }
}
