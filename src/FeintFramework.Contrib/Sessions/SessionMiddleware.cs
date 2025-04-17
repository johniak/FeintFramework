using FeintFramework.Contrib.Sessions.Models;
using FeintFramework.Config;
using FeintFramework.Http;
using FeintFramework.Middleware;
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
            session = Session.Objects.FirstOrDefault(s => s.SessionKey == sessionId);
        }
        if (session == null)
        {
            session = new Session();
            session.Save();
        }
        var sessionStore = new SessionStore(session);
        request.AdditionalData[SessionConsts.REQUEST_SESSION_STORE_KEY] = sessionStore;
        var respnonse = handler(request);
        respnonse.Cookies.Append(sessionCookieName, session.SessionKey, Configurator.Settings.SessionCookieOptions());
        return respnonse;
    }
}