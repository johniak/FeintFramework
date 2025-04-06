using FeintFramework.Core.Config;
using FeintFramework.Core.Http;
using FeintFramework.Core.Http.Exceptions;

namespace FeintFramework.Core.Middleware;


internal class ErrorHandlingMiddleware : BaseMiddleware
{
    public ErrorHandlingMiddleware(RequestHandler handler) : base(handler)
    {
    }

    public override FeintHttpResponse HandleRequest(FeintHttpRequest request)
    {
        try
        {
            return handler(request);
        }
        catch (Http404 e)
        {
            return Configurator.Settings.RootUrlPatterns.handler404(request, e);
        }
    }
}