
using FeintFramework.Http;

namespace FeintFramework.Middleware;
public abstract class BaseMiddleware
{
    protected RequestHandler handler;
    public BaseMiddleware(RequestHandler handler){
        this.handler = handler;
    }
    abstract public FeintHttpResponse HandleRequest(FeintHttpRequest request);
}