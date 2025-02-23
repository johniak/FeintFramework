
using FeintFramework.Core.Http;

public abstract class BaseMiddleware
{
    RequestHandler handler;
    BaseMiddleware(RequestHandler handler){
        this.handler = handler;
    }
    abstract public FeintHttpResponse HandleRequest(FeintHttpRequest request);
}