using System;
using System.Net;
using System.IO;
using FeintSDK;
using Feint.Core;

namespace Feint
{
    /// <summary>
    /// Simple asynchronus HTTP server.
    /// </summary>
    class DebugServer : Server
    {
        HttpListener _listener;
        public DebugServer(String address)
            : base(address)
        {


        }
        /// <summary>
        /// Starts HTTP server on selected address and port. Server starts infinity loop.
        /// Each request is asychronus
        /// </summary>
        public override void Start()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://" + Address + "/");
            _listener.Start();
            Log.I("Feint server started at: " + Address);
            var result = _listener.BeginGetContext(ListenerCallback, _listener);
            while (true)
            {
                result.AsyncWaitHandle.WaitOne();
                result = _listener.BeginGetContext(ListenerCallback, _listener);
            }
        }

        /// <summary>
        /// Request handling
        /// </summary>
        /// <param name="result"></param>
        private void ListenerCallback(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            var context = listener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;
            string requestBody;
            using (var reader = new StreamReader(request.InputStream,
                                     request.ContentEncoding))
            {
                requestBody = reader.ReadToEnd();
            }
            var req = new Request(request.Url.LocalPath)
            {
                Body = requestBody,
                ContentType = request.ContentType,
                MethodString = request.HttpMethod
            };
            for (var i = 0; i < request.Cookies.Count; i++)
            {
                var cookie = request.Cookies[i];
                req.Cookies.Set(cookie);
            }
            foreach (var key in request.Headers.AllKeys)
            {
                var value = request.Headers.GetValues(key);
                if (value != null)
                    req.Headers.Add(key, value[0]);
            }

            var res = HandleRequest(req);
            foreach (var cookie in res.Cookies)
            {
                response.SetCookie(cookie);
            }
            if (res.IsRedirect)
            {
                response.Redirect(res.RedirectUrl);
                response.OutputStream.Close();
                return;
            }
            response.StatusCode = res.Status;
            response.ContentLength64 = res.Data.Length;
            var output = response.OutputStream;
            output.Write(res.Data, 0, res.Data.Length);
            output.Close();
        }
    }
}
