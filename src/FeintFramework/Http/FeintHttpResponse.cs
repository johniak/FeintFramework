using Microsoft.AspNetCore.Http;

namespace FeintFramework.Http;
public class FeintHttpResponse
{
    public int StatusCode { get; set; } = 200;
    public string ContentType { get; set; } = "text/plain";
    public string Content { get; set; } = "";
    public IHeaderDictionary Headers { get; protected set; } = new HeaderDictionary();

    public FeintFrameworkResponseCookies Cookies { get; protected set; } = new FeintFrameworkResponseCookies();

}