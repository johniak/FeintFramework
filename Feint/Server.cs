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
namespace Feint
{
    class DebugServer
    {
        HttpListener listener;
        public DebugServer(String address)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(address);
            listener.Start();
            Log.I("Fint server started at: " + address);
          //  Console.WriteLine("Fint server started at: " + adress);
            IAsyncResult result = listener.BeginGetContext(new AsyncCallback(listenerCallback), listener);
            while (true)
            {
                result.AsyncWaitHandle.WaitOne();
                result = listener.BeginGetContext(new AsyncCallback(listenerCallback), listener);
            }
        }
        int trololo = 0;
        private void listenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            try
            {

                Response res = null;
                Request req = new Request(request.Url.LocalPath.ToString());
                req.Method = request.HttpMethod;
                req.Session = new Session();


                string text = null;

                using (var reader = new StreamReader(request.InputStream,
                                         request.ContentEncoding))
                {
                    text = reader.ReadToEnd();
                }
                //text = text.Replace("\r", "");

                if (text.Length > 0)
                {
                    if (!text.Contains("WebKitFormBoundary"))
                    {
                        var postTabs = text.Split('&');
                        foreach (var p in postTabs)
                        {
                            req.POST.Add(p.Split('=')[0], p.Split('=')[1]);
                        }
                    }else
                    {

                        string[] postTab = text.Replace("\r","").Split('\n');
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
                                req.POST.Add(postName, postTab[i]);

                        }
                    }
                }
                if (request.Url.LocalPath.ToString().StartsWith("/" + Settings.StaticFolder))
                {

                    if (request.Url.LocalPath.ToString().Contains("./") || request.Url.LocalPath.ToString().Contains(".."))
                        res = null;
                    else
                    {
                        try
                        {
                            FileStream fs = new FileStream("FeintSite/" + request.Url.LocalPath.Substring(1), FileMode.Open, FileAccess.Read);
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, (int)fs.Length);
                            res = new Response(buffer);
                            fs.Close();
                        }
                        catch
                        {

                        }
                    }
                }
                else
                    for (int i = 0; i < Settings.Urls.Count; i++)
                    {
                        var match = Regex.Match(request.Url.LocalPath.ToString(), Settings.Urls[i].UrlMatch);

                        if (match.Success)
                        {
                            setNonPublicSetProperty(req, req.GetType(), "variables", match.Groups);



                            setSesion(request, response, req);
                            res = Settings.Urls[i].View(req);
                            var redirect = res.GetType().GetField("redirectUrl",
                             BindingFlags.NonPublic |
                             BindingFlags.Instance);
                            String url = (String)redirect.GetValue(res);
                            if (url != null)
                            {
                                response.Redirect(url);
                                response.Close();
                                return;
                            }
                            break;
                        }
                    }
                if (res != null)
                {
                    response.ContentLength64 = res.Data.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(res.Data, 0, res.Data.Length);
                    output.Close();
                }
                else
                {
                    string responseString = "<HTML><BODY>404!!!</BODY></HTML>";
                    byte[] buffer = System.Text.Encoding.GetEncoding(1252).GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }


            }
            catch (IOException e)
            {
                Log.D(e);
               // Console.WriteLine(e);
                string responseString = "<HTML><BODY>Im n00b- internal server erron- 503!!!</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.GetEncoding(1252).GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private static void setSesion(HttpListenerRequest request, HttpListenerResponse response, Request req)
        {
            var c = request.Cookies["session"];
            if (c == null)
                response.Cookies.Add(new Cookie("session", req.Session.Start(), "/"));
            else
            {

                string key = req.Session.Start(c.Value);

                if (c.Value.CompareTo(key) != 0)
                {
                    try
                    {
                        response.Cookies["session"].Value = key;
                    }
                    catch (NullReferenceException e)
                    {
                        response.Cookies.Add(new Cookie("session", key, "/"));
                    }


                }

            }
        }
        public void setNonPublicSetProperty(Object obj, Type t, string name, dynamic value)
        {
            var properties = t.GetProperties();
            t.GetProperty(name).SetValue(obj, value);
        }
    }
}
