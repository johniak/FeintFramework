using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using FeintSDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;

namespace FeintSDK.Server
{
    public class Server : BaseServer
    {
        protected String Address;
        protected int Port;

        public Server(String address,int port)
        {
            Address = address;
            Port = port;
        }

        public void Start()
        {
            var host = new WebHostBuilder()
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Parse(this.Address), this.Port);
            })
            .Configure(app =>
            {
                app.Run(handleNewRequest);
            })
            .Build();
            host.Run();
        }
        
        protected async Task handleNewRequest(HttpContext context)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var request = createSdkRequest(context.Request);
                var response = HandleRequest(request);
                fillHttpResponse(context.Response, response);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync(ex.ToString());
            }
            finally
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"[{context.Request.Method}][{DateTime.Now}] \"{context.Request.Path} {context.Response.StatusCode}\" {elapsedMs}ms");
            }
        }
        protected Request createSdkRequest(HttpRequest req)
        {
            var sdkRequest = new Request(req.Path);
            sdkRequest.Body = "";
            sdkRequest.ContentType = req.ContentType;
            sdkRequest.MethodString = req.Method;
            if(req.HasFormContentType)
            {
                foreach (var formElement in req.Form)
                {
                    sdkRequest.FormData.Add(formElement.Key, formElement.Value);
                }
            }
            else
            {

                StreamReader reader = new StreamReader(req.Body);
                sdkRequest.Body = reader.ReadToEnd();
            }
            List<Cookie> cookieList = new List<Cookie>();
            foreach (var cookie in req.Cookies)
            {
                cookieList.Add(new Cookie(cookie.Key, cookie.Value));
            }
            sdkRequest.Cookies.AddAll(cookieList);
            foreach (var header in req.Headers)
            {
                sdkRequest.Headers.Add(header.Key, header.Value);
            }
            return sdkRequest;
        }
        protected void fillHttpResponse(HttpResponse httpResponse, Response response)
        {
            foreach (var cookie in response.Cookies)
            {
                var options = new CookieOptions();
                if(cookie.Domain.Length!=0)
                     options.Domain = cookie.Domain;
                if(cookie.Expires.Ticks!=0)
                    options.Expires = cookie.Expires;
                options.HttpOnly = cookie.HttpOnly;
                options.Path = cookie.Path;
                options.Secure = cookie.Secure;
                httpResponse.Cookies.Append(cookie.Name, cookie.Value, options);
            }
            foreach (var header in response.Headers)
            {
                httpResponse.Headers.Add(header.Key, header.Value);
            }
            httpResponse.StatusCode = response.Status;
            httpResponse.ContentType = response.ContentType;
            httpResponse.Body.Write(response.Data, 0, response.Data.Length);
        }
    }
}
