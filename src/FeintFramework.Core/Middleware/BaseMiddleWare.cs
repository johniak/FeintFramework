
using FeintFramework.Core.Http;

public abstract class BaseMiddleware
{
    Func<FeintHttpRequest, FeintHttpResponse> handler;
    BaseMiddleware(Func<FeintHttpRequest, FeintHttpResponse> handler){
        this.handler = handler;
    }
    abstract public FeintHttpResponse HandleRequest(FeintHttpRequest request);
}