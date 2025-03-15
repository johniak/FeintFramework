using FeintFramework.Contrib.Sessions.Models;
using FeintFramework.Core.Config;
using FeintFramework.Core.Http;
using FeintFramework.Core.Middleware;
using FeintFramework.Contrib.Sessions;
using FeintFramework.Db;
using LinqToDB;

namespace FeintFramework.Contrib.Auth;

public class AuthMiddleware : BaseMiddleware
{
    public AuthMiddleware(RequestHandler handler) : base(handler)
    {
    }

    public override FeintHttpResponse HandleRequest(FeintHttpRequest request)
    {
        var session = request.Session();
        const string userIdKey = AuthConsts.SESSION_USER_ID_KEY;
        if (session == null)
        {
            throw new Exception("No session, remember that session middleware need to be before.");
        }

        if (session.ContainsKey(userIdKey))
        {
            var userId = (int)session[userIdKey];
            var authBackend = Configurator.Settings.AuthBackend(request);
            var user = authBackend.GetUser(userId);
            if (user == null)
            {
                session.Remove(userIdKey);
            }
            else
            {
                request.AdditionalData[AuthConsts.REQUEST_USER_STORE_KEY] = user;
            }
        }
        var respnonse = handler(request);
        return respnonse;
    }
}