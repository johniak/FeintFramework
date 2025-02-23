using System.Text;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Core.Http;

public class KestrelServerHandler : BaseServerHandler
{
    public KestrelServerHandler(RequestHandler handler) : base(handler)
    {
    }

    public override object? HandleRequest(object request)
    {
        var context = (HttpContext)request;
        handleRequest(context);
        return null;
    }

    protected void handleRequest(HttpContext context)
    {
        var feintRequest = convertToFeintRequest(context.Request);
        convertToKestrelResponse(routerHandler(feintRequest), context);
    }
    protected FeintHttpRequest convertToFeintRequest(HttpRequest kestrelRequest)
    {
        var feintRequest = new FeintHttpRequest()
        {
            Body = kestrelRequest.Body,
            ContentLength = kestrelRequest.ContentLength,
            ContentType = kestrelRequest.ContentType,
            Headers = kestrelRequest.Headers,
            Host = kestrelRequest.Host.ToString(),
            Method = kestrelRequest.Method,
            Path = kestrelRequest.Path,
            Protocol = kestrelRequest.Protocol,
            QueryString = kestrelRequest.QueryString.ToString(),
            Query = kestrelRequest.Query,
            Scheme = kestrelRequest.Scheme
        };

        return feintRequest;
    }

    protected void convertToKestrelResponse(FeintHttpResponse feintResponse, HttpContext context)
    {
        var kestrelResponse = context.Response;
        kestrelResponse.StatusCode = feintResponse.StatusCode;
        kestrelResponse.ContentType = feintResponse.ContentType;

        for (var i = 0; i < feintResponse.Headers.Count; i++)
        {
            var header = feintResponse.Headers.ElementAt(i);
            kestrelResponse.Headers[header.Key] = header.Value;
        }
        kestrelResponse.Body.Write(Encoding.UTF8.GetBytes(feintResponse.Content));
    }
}
