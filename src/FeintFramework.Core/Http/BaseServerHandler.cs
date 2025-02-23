using FeintFramework.Core.Config;

namespace FeintFramework.Core.Http;

public abstract class BaseServerHandler
{
    protected Func<FeintHttpRequest, FeintHttpResponse> handler;
    public BaseServerHandler(Func<FeintHttpRequest, FeintHttpResponse> handler)
    {
        this.handler = handler;
    }

    public virtual List<Type> GetMiddlewares()
    {
        return new List<Type>(Configurator.Settings.Middlewares);
    }
    public virtual FeintHttpResponse HandleNestedRequest(FeintHttpRequest request, List<Type> middlewares)
    {
        middlewares.Reverse();
        var handler = this.handler;
        foreach (var middlewareType in middlewares)
        {
            var middleware = (BaseMiddleware)Activator.CreateInstance(middlewareType, new object[] { handler })!;
            handler = middleware.HandleRequest;
        }
        return handler(request);
    }
    public virtual object? HandleRequest(object request)
    {
        var middlewares = GetMiddlewares();
        return HandleNestedRequest((FeintHttpRequest)request, middlewares);


    }
    protected abstract object? handleRequest(object request);
}
