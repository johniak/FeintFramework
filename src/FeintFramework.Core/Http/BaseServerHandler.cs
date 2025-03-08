using FeintFramework.Core.Config;
using FeintFramework.Core.Middleware;

namespace FeintFramework.Core.Http;

public abstract class BaseServerHandler
{
    protected RequestHandler routerHandler;
    public BaseServerHandler(RequestHandler rourterHandler)
    {
        this.routerHandler = rourterHandler;
    }

    protected virtual RequestHandler handler
    {
        get
        {
            var middlewares = new List<Type>(Configurator.Settings.Middlewares);
            middlewares.Reverse();
            var handler = this.routerHandler;
            foreach (var middlewareType in middlewares)
            {
                var middleware = (BaseMiddleware)Activator.CreateInstance(middlewareType, new object[] { handler })!;
                handler = middleware.HandleRequest;
            }
            return handler;
        }
    }

    public abstract object? HandleRequest(object request);
}
