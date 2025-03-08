
using FeintFramework.Core.Http;

namespace FeintFramework.Core.Middleware;
public abstract class BaseMiddleware
{
    protected RequestHandler handler;
    public BaseMiddleware(RequestHandler handler){
        this.handler = handler;
    }
    abstract public FeintHttpResponse HandleRequest(FeintHttpRequest request);
}