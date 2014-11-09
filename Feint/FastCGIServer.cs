using FastCGI;
using FeintORM;
using System;
using System.Net;
using System.Text;


namespace Feint
{
    /// <summary>
    /// FastCGI server builded on pure quality FastCGI server library. Doesn't support response headers.
    /// </summary>
    class FastCgiServer : Core.Server
    {
        private readonly Options _config;

        public FastCgiServer(String address) : base(address)
        {
            var splittedAddress = address.Split(':');
            _config = new Options
            {
                Bind = BindMode.CreateSocket,
                EndPoint = new IPEndPoint(IPAddress.Parse(splittedAddress[0]), short.Parse(splittedAddress[1])),
                OnError = Log.E
            };
        }

        public override void Start()
        {
            Log.I("FastCGI started at: " + Address);
            Server.Start(HandleRequest, _config);
        }

        private void HandleRequest(Request request, Response response)
        {
            var req = new FeintSDK.Request(request.RequestURI.Value)
            {
                MethodString = request.RequestMethod.Value,
                Body = Encoding.UTF8.GetString(request.Stdin.GetContents()),
                ContentType = request.ContentType.Value.Name
            };
            foreach (var cookie in request.Cookies)
            {
                req.Cookies.Set(cookie.Value);
            }
            req.Session = new FeintSDK.Session();

            foreach (var header in request.Headers)
            {
                req.Headers.Add(header.Key.ToString(), header.Value);
            }
            // ReSharper disable once RedundantBaseQualifier
            var res = base.HandleRequest(req);
            if (res.IsRedirect)
            {
                response.SeeOtherRedirect(res.RedirectUrl);
                return;
            }
            response.ResponseStatus = res.Status;
            response.Put(res.Data);
            foreach (var cookie in res.Cookies)
            {
                response.SetCookie(cookie);
            }
        }

    }

}
