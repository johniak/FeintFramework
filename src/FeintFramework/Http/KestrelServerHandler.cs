using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace FeintFramework.Http;

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
        convertToKestrelResponse(handler(feintRequest), context);
    }
    protected FeintHttpRequest convertToFeintRequest(HttpRequest kestrelRequest)
    {
        
        var postList = new List<(string Key, StringValues Values)>();
        try
        {
            foreach (var item in kestrelRequest.Form)
            {
                postList.Add((item.Key, item.Value));
            }
        }
        catch (InvalidOperationException)
        {

        }
        var postParameters = new QueryDict(postList);
        var getList = new List<(string Key, StringValues Values)>();
        foreach (var item in kestrelRequest.Query)
        {
            getList.Add((item.Key, item.Value));
        }
        var getParameters = new QueryDict(getList);
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
            Scheme = kestrelRequest.Scheme,
            Cookies = kestrelRequest.Cookies,
            Post = postParameters,
            Get = getParameters,
        };

        return feintRequest;
    }

    protected void convertToKestrelResponse(FeintHttpResponse feintResponse, HttpContext context)
    {
        var kestrelResponse = context.Response;
        kestrelResponse.StatusCode = feintResponse.StatusCode;
        kestrelResponse.ContentType = feintResponse.ContentType;
        foreach (var cookie in feintResponse.Cookies)
        {
            kestrelResponse.Cookies.Append(cookie.Key, cookie.Value, cookie.Options);
        }

        for (var i = 0; i < feintResponse.Headers.Count; i++)
        {
            var header = feintResponse.Headers.ElementAt(i);
            kestrelResponse.Headers[header.Key] = header.Value;
        }
        kestrelResponse.Body.Write(Encoding.UTF8.GetBytes(feintResponse.Content));
    }
}
