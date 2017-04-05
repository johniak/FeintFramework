using System;
using FeintSDK;
using FeintSDK.Middlewares;
using FeintServer.Core;
namespace app
{
    class Program
    {
        public static Response Index(Request request)
        {
            var response = new Response(request,"Hi! Im your dashboard");
            return response;
        }
        
        static void Main(string[] args)
        {
            Settings.Urls.Add(new Url(@"^/$", Program.Index));
            Settings.Midelwares.Add(typeof(CookieSessionMiddleware));
            Server s = new Server("0.0.0.0:5000");
            s.Start();
        }
    }
}
