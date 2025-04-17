using FeintFramework.Http;
using FeintFramework.Routing;
using Microsoft.AspNetCore.Http;

namespace FeinFramework.Core.Routing.Tests;

class NestedUrlPatterns : UrlPatterns
{

    RequestHandler handler;
    public NestedUrlPatterns(RequestHandler handler)
    {
        this.handler = handler;
    }
    public override List<UrlPattern> Urls => new List<UrlPattern>
    {
        new UrlPattern("/nested", this.handler),
    };
}

// class TestUrlPatterns : UrlPatterns
// {
//     RequestHandler handler;
//     RequestHandler nestedHandler;
//     public TestUrlPatterns(RequestHandler handler, RequestHandler nestedHandler)
//     {
//         this.handler = handler;
//         this.nestedHandler = nestedHandler;
//     }
//     public override List<UrlPattern> Urls => new List<UrlPattern>
//     {
//         new UrlPattern("/test", this.handler),
//         new UrlPattern("/foo", new NestedUrlPatterns(this.nestedHandler).Urls),
//     };
// }



// [TestClass]
// public sealed class TestRouter
// {

//     [TestMethod]
//     public void HandleRequest_CallsHandler_WhenMatchedUrl()
//     {
//         TestUrlPatterns urlPatterns = new TestUrlPatterns((request) =>
//         {
//             return new FeintHttpResponse
//             {
//                 StatusCode = 200,
//                 Content = "OK"
//             };

//         },
//         (request) =>
//         {
//             return new FeintHttpResponse
//             {
//                 StatusCode = 200,
//                 Content = "NESTED"
//             };
//         }
//         );
//         Router router = new Router(urlPatterns);
//         FeintHttpRequest request = new FeintHttpRequest
//         {
//             ContentType = "text/plain",
//             Body = new MemoryStream(),
//             ContentLength = 0,
//             Host = "localhost",
//             Protocol = "HTTP/1.1",
//             QueryString = "",
//             Query = new QueryCollection(),
//             Scheme = "http",
//             Path = "/test",
//             Method = "GET",
//             Headers = new HeaderDictionary(),
//             Cookies = new RequestCookieCollection()
//         };
//         var response = router.HandleRequest(request);
//         Assert.AreEqual(200, response.StatusCode);
//         Assert.AreEqual("OK", response.Content);
//     }

//     [TestMethod]
//     public void HandleRequest_DontCallsHandler_WhenNotMatchedUrl()
//     {
//         TestUrlPatterns urlPatterns = new TestUrlPatterns((request) =>
//         {
//             return new FeintHttpResponse
//             {
//                 StatusCode = 200,
//                 Content = "OK"
//             };
//         },
//         (request) =>
//         {
//             return new FeintHttpResponse
//             {
//                 StatusCode = 200,
//                 Content = "NESTED"
//             };
//         });
//         Router router = new Router(urlPatterns);
//         FeintHttpRequest request = new FeintHttpRequest
//         {
//             ContentType = "text/plain",
//             Body = new MemoryStream(),
//             ContentLength = 0,
//             Host = "localhost",
//             Protocol = "HTTP/1.1",
//             QueryString = "",
//             Query = new QueryCollection(),
//             Scheme = "http",
//             Path = "/test2",
//             Method = "GET",
//             Headers = new HeaderDictionary()
//         };
//         var response = router.HandleRequest(request);
//         Assert.AreEqual(404, response.StatusCode);
//     }

//     [TestMethod]
//     public void HandleRequest_CallsHandler_WhenMatchedNestedUrl()
//     {
//         TestUrlPatterns urlPatterns = new TestUrlPatterns((request) =>
//         {
//             return new FeintHttpResponse
//             {
//                 StatusCode = 200,
//                 Content = "OK"
//             };

//         },
//         (request) =>
//         {
//             return new FeintHttpResponse
//             {
//                 StatusCode = 200,
//                 Content = "NESTED"
//             };
//         }
//         );
//         Router router = new Router(urlPatterns);
//         FeintHttpRequest request = new FeintHttpRequest
//         {
//             ContentType = "text/plain",
//             Body = new MemoryStream(),
//             ContentLength = 0,
//             Host = "localhost",
//             Protocol = "HTTP/1.1",
//             QueryString = "",
//             Query = new QueryCollection(),
//             Scheme = "http",
//             Path = "/foo/nested",
//             Method = "GET",
//             Headers = new HeaderDictionary()
//         };
//         var response = router.HandleRequest(request);
//         Assert.AreEqual(200, response.StatusCode);
//         Assert.AreEqual("NESTED", response.Content);
//     }
// }
