using System;
using System.Collections.Generic;
using FeintApi;
using FeintSDK;
using FeintSDK.Server;
using System.Reflection;
using System.Linq;
namespace FeintSite
{

    class Program
    {
        public static Response Index(Request request)
        {
            var response = new ApiResponse("true");
            return response;
        }

        public static Response Webhook(Request request)
        {
            Console.WriteLine(request.Body);
            var response = new ApiResponse("false");
            return response;
        }

        static void Main(string[] args)
        {
            Settings.Urls.Add(new Url(@"^/$", Program.Index));
            Settings.Urls.Add(new Url(@"^/webhook/$", Program.Webhook));
            Server s = new Server("0.0.0.0", 5000);
            s.Start(); 
        }
    }
}
