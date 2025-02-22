using FeintFramework.Core.Http;
using FeintFramework.Core.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FeinFramework.Core.Routing.Tests;


class TestUrlPatterns : UrlPatterns
{
    Func<FeintHttpRequest, FeintHttpResponse> handler;
    public TestUrlPatterns(Func<FeintHttpRequest, FeintHttpResponse> handler)
    {
        this.handler = handler;
    }
    public override List<UrlPattern> Urls => new List<UrlPattern>
    {
        new UrlPattern("/test", this.handler)
    };
}


[TestClass]
public sealed class TestRouter
{
    
    [TestMethod]
    public void HandleRequest_CallsHandler_WhenMatchedUrl()
    {
        TestUrlPatterns urlPatterns = new TestUrlPatterns((request) =>
        {
            return new FeintHttpResponse
            {
                StatusCode = 200,
                Content = "OK"
            };
        });
        Router router = new Router(urlPatterns);
        FeintHttpRequest request = new FeintHttpRequest
        {
            ContentType="text/plain",
            Body = new MemoryStream(),
            ContentLength = 0,
            Host = "localhost",
            Protocol = "HTTP/1.1",
            QueryString = "",
            Query = new QueryCollection(),
            Scheme = "http",
            Path = "/test",
            Method = "GET",
            Headers = new HeaderDictionary()
        };
        var response = router.HandleRequest(request);
        Assert.AreEqual(200, response.StatusCode);
        Assert.AreEqual("OK", response.Content);
    }

        [TestMethod]
    public void HandleRequest_DontCallsHandler_WhenNotMatchedUrl()
    {
        TestUrlPatterns urlPatterns = new TestUrlPatterns((request) =>
        {
            return new FeintHttpResponse
            {
                StatusCode = 200,
                Content = "OK"
            };
        });
        Router router = new Router(urlPatterns);
        FeintHttpRequest request = new FeintHttpRequest
        {
            ContentType="text/plain",
            Body = new MemoryStream(),
            ContentLength = 0,
            Host = "localhost",
            Protocol = "HTTP/1.1",
            QueryString = "",
            Query = new QueryCollection(),
            Scheme = "http",
            Path = "/test2",
            Method = "GET",
            Headers = new HeaderDictionary()
        };
        var response = router.HandleRequest(request);
        Assert.AreEqual(404, response.StatusCode);
    }
}
