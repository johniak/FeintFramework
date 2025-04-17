using FeintFramework.Config;
using FeintFramework.Http;
using FeintFramework.Http.Exceptions;

namespace FeintFramework.Middleware;


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
        catch(Exception e){
            return Configurator.Settings.RootUrlPatterns.handler500(request, e);
        }
    }
}