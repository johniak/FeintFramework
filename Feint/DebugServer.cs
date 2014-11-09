using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using FeintSDK;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;
using HttpUtils;
using System.Web;
using Feint.Core;
using Cookie = FeintSDK.Cookie;

namespace Feint
{
    class DebugServer : Server
    {
        HttpListener listener;
        public DebugServer(String address)
            : base(address)
        {


        }

        public override void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://" + Address + "/");
            listener.Start();
            Log.I("Fint server started at: " + Address);
            //  Console.WriteLine("Fint server started at: " + adress);
            IAsyncResult result = listener.BeginGetContext(new AsyncCallback(listenerCallback), listener);
            while (true)
            {
                result.AsyncWaitHandle.WaitOne();
                result = listener.BeginGetContext(new AsyncCallback(listenerCallback), listener);
            }
        }

        private void listenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string requestBody = null;
            using (var reader = new StreamReader(request.InputStream,
                                     request.ContentEncoding))
            {
                requestBody = reader.ReadToEnd();
            }
            var req = new Request(request.Url.LocalPath.ToString())
            {
                Body = requestBody,
                ContentType = request.ContentType,
                MethodString = request.HttpMethod
            };
            //foreach (var cookie in request.Cookies)
            for(var i =0;i<request.Cookies.Count;i++)
            {
                var cookie = request.Cookies[i];
                
                req.Cookies.Set(new Cookie(){Name = cookie.Name,ExperiationDate = cookie.Expires,});
            }
            foreach (var key in request.Headers.AllKeys)
            {
                req.Headers.Add(key, request.Headers.GetValues(key)[0]);
            }

            var res = base.HandelRequest(req);
            response.StatusCode = res.Status;
            response.ContentLength64 = res.Data.Length;
            var output = response.OutputStream;
            output.Write(res.Data, 0, res.Data.Length);
            output.Close();
        }

        public Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


    }
}
