namespace FeintFramework.Core.Http;

public abstract class BaseServerHandler
{
    protected Func<FeintHttpRequest, FeintHttpResponse> handler;
    public BaseServerHandler(Func<FeintHttpRequest, FeintHttpResponse> handler)
    {
        this.handler = handler;
    }

    public abstract object HandleRequest(object request);
}
