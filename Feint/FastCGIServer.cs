using FastCGI;
using FeintORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Feint
{
    class FastCGIServer
    {
        public FastCGIServer(String address)
        {
            var splittedAddress = address.Split(':');
            Options config = new Options();
            config.Bind = BindMode.CreateSocket;
            config.EndPoint = new IPEndPoint(IPAddress.Parse(splittedAddress[0]), short.Parse(splittedAddress[1]));
            config.OnError = Log.E;
            Log.I("FastCGI started at: " + address);
            Server.Start(HandleRequest, config);
        }
        void HandleRequest(Request request, Response response)
        {
            Log.E(request.RequestURI.Value);
            //            // receive HTTP content
            //            byte[] content = request.Stdin.GetContents();

            //            // access server variables
            //            string serverSoftware = request.ServerSoftware.GetValueOrDefault();
            //            string method = request.RequestMethod.Value;

            //            // access HTTP headers
            //            string userAgent = request.Headers[RequestHeader.HttpUserAgent];
            //            string cookieValue = request.GetCookieValue("Keks").GetValueOrDefault();

            //            // set HTTP headers
            //            response.SetHeader(ResponseHeader.HttpExpires,
            //                               Response.ToHttpDate(DateTime.Now.AddDays(1.0)));
            //            response.SetCookie(new Cookie("Keks", "yummy"));



            //            // send HTTP content
            //            response.PutStr(
            //                @"<html>
            //                   <body>
            //                    <p>Chrome why?</p>
            //                    <p>Server: " + serverSoftware + @"</p>
            //                    <p>User Agent: " + userAgent + @"</p>
            //                    <p>Received cookie value: " + cookieValue + @"</p>
            //                    <p>Content length as read: " + content.Length + @"</P>
            //                    <p>Request method: " + method + @"</p>
            //                   </body>
            //                  </html>"
            //                );
            FeintSDK.Response res = null;
            FeintSDK.Request req = new FeintSDK.Request(request.RequestURI.Value);
            req.Method = request.RequestMethod.Value;
            req.Session = new FeintSDK.Session();
            string text = null;

            text = System.Text.Encoding.UTF8.GetString(request.Stdin.GetContents());
            if (text.Length > 0)
            {
                if (!text.Contains("WebKitFormBoundary"))
                {
                    var postTabs = text.Split('&');
                    foreach (var p in postTabs)
                    {
                        req.POST.Add(p.Split('=')[0], p.Split('=')[1]);
                    }
                }
                else
                {

                    string[] postTab = text.Replace("\r", "").Split('\n');
                    String postName = "";
                    for (int i = 0; i < postTab.Length; i++)
                    {
                        if (postTab[i].StartsWith("------"))
                            continue;
                        if (postTab[i].StartsWith("Content-Disposition: form-data;"))
                        {

                            postName = postTab[i].Substring(38, postTab[i].LastIndexOf("\"") - 38);
                            continue;
                        }
                        if (postTab[i].Length > 0)
                        {
                            req.POST.Add(postName, postTab[i]);
                        }

                    }
                }
            }

            if (req.Url.StartsWith("/" + FeintSDK.Settings.StaticFolder))
            {

                if (req.Url.Contains("./") || req.Url.ToString().Contains(".."))
                    res = null;
                else
                {
                    try
                    {
                        FileStream fs = new FileStream("FeintSite/" + req.Url.Substring(1), FileMode.Open, FileAccess.Read);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        res = new FeintSDK.Response(buffer);
                        fs.Close();
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                FeintSDK.RequestMethod actualMethod = FeintSDK.RequestMethod.POST;
                switch (request.RequestMethod.Value)
                {
                    case "GET":
                        actualMethod = FeintSDK.RequestMethod.GET;
                        break;
                    case "POST":
                        actualMethod = FeintSDK.RequestMethod.POST;
                        break;
                    case "PUT":
                        actualMethod = FeintSDK.RequestMethod.PUT;
                        break;
                    case "DELETE":
                        actualMethod = FeintSDK.RequestMethod.DELETE;
                        break;
                }
                for (int i = 0; i < FeintSDK.Settings.Urls.Count; i++)
                {
                    var match = Regex.Match(req.Url.ToString(), FeintSDK.Settings.Urls[i].UrlMatch);

                    if (match.Success && (FeintSDK.Settings.Urls[i].Method == FeintSDK.RequestMethod.ALL || actualMethod == FeintSDK.Settings.Urls[i].Method))
                    {
                        setNonPublicSetProperty(req, req.GetType(), "variables", match.Groups);



                        setSesion(request, response, req);
                        res = FeintSDK.Settings.Urls[i].View(req);
                        var redirect = res.GetType().GetField("redirectUrl",
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);
                        String url = (String)redirect.GetValue(res);
                        if (url != null)
                        {
                            response.SeeOtherRedirect(url);
                            // response.Redirect(url);

                            // response.Close();
                            return;
                        }
                        break;
                    }
                }
            }
            if (res != null)
            {

                //response.ContentLength64 = res.Data.Length;
                //System.IO.Stream output = response.OutputStream;
                //output.Write(res.Data, 0, res.Data.Length);
                //output.Close();
                response.Put(res.Data);
            }
            else
            {
                //FeintSDK.
                response.PutStr("<HTML><BODY>404!!!</BODY></HTML>");
                //string responseString = "<HTML><BODY>404!!!</BODY></HTML>";
                //byte[] buffer = System.Text.Encoding.GetEncoding(1252).GetBytes(responseString);
                //response.ContentLength64 = buffer.Length;
                //System.IO.Stream output = response.OutputStream;
                //output.Write(buffer, 0, buffer.Length);
                //output.Close();
            }
        }
        private static void setSesion(Request request, Response response, FeintSDK.Request req)
        {
            if (request.Cookies.ContainsKey("session"))
            {
                var c = request.Cookies["session"];
                string key = req.Session.Start(c.Value);

                if (c.Value.CompareTo(key) != 0)
                {
                    try
                    {
                        response.SetCookie(new Cookie("session", key, "/"));
                    }
                    catch (NullReferenceException e)
                    {
                        response.SetCookie(new Cookie("session", key, "/"));
                    }
                }
            }
            else
                response.SetCookie(new Cookie("session", req.Session.Start(), "/"));
        }
        public void setNonPublicSetProperty(Object obj, Type t, string name, dynamic value)
        {
            var properties = t.GetProperties();
            t.GetProperty(name).SetValue(obj, value);
        }
    }
}
