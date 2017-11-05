using System;
using System.Collections.Generic;
using FeintApi;
using FeintSDK;
using FeintServer.Core;
using System.Reflection;
using System.Linq;
namespace FeintSite
{

    class Program
    {
        public static Response Index(Request request)
        {
            var response = new ApiResponse("{\"Hello\":\"World\"}");
            return response;
        }

        static void Main(string[] args)
        {
            Settings.Urls.Add(new Url(@"^/$", Program.Index));
            Server s = new Server("0.0.0.0", 5000);
            s.Start(); 
        }
    }
}
