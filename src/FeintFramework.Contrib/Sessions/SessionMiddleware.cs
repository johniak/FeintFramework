using FeintFramework.Contrib.Sessions.Models;
using FeintFramework.Core.Config;
using FeintFramework.Core.Http;
using FeintFramework.Core.Middleware;
using FeintFramework.Db;
using LinqToDB;

namespace FeintFramework.Contrib.Sessions;

public class SessionMiddleware : BaseMiddleware
{
    public SessionMiddleware(RequestHandler handler) : base(handler)
    {
    }

    public override FeintHttpResponse HandleRequest(FeintHttpRequest request)
    {
        var sessionCookieName = Configurator.Settings.SessionCookieName();
        Session? session = null;
        if (request.Cookies.ContainsKey(sessionCookieName))
        {
            var sessionId = request.Cookies[sessionCookieName];
            session = Session.Objects.First(s => s.SessionKey == sessionId);
        }
        if (session == null)
        {
            session = new Session();
            session.Save();
        }
        var respnonse = handler(request);
        respnonse.Cookies.Append(sessionCookieName, session.SessionKey, Configurator.Settings.SessionCookieOptions());
        return respnonse;
    }
}