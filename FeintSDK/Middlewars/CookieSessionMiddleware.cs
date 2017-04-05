using System;
using System.Net;

namespace FeintSDK.Middlewares
{
    public class CookieSessionMiddleware : Midelware
    {
        string SessionKey = null;
        public override Request ModifyRequest(Request request, Url urlApp)
        {
            request.Session = new Session();
            this.SessionKey = SetSesion(request);
            return request;
        }
        public override Response ModifyResponse(Request request, Response response, Url urlApp)
        {
            if (response != null && this.SessionKey != null)
            {
                response.Cookies.SetCookie(new Cookie("session", request.Session.Key, "/"));
            }
            return response;
        }

        private static string SetSesion(Request request)
        {
            if (!request.Cookies.IsCookieExist("session"))
            {
                return request.Session.Start();
            }
            var c = request.Cookies["session"];
            request.Session.Start(c.Value);
            return null;
        }
    }
}