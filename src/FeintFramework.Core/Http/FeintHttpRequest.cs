using System.Text.Json;
using FeintFramework.Core.Contrib.Sessions;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Core.Http;
static class HttpMethods
{
    public const string Get = "GET";
    public const string Post = "POST";
    public const string Put = "PUT";
    public const string Delete = "DELETE";
    public const string Patch = "PATCH";
    public const string Options = "OPTIONS";
    public const string Head = "HEAD";
    public const string Trace = "TRACE";
    public const string Connect = "CONNECT";
}

public class FeintHttpRequest
{

    public string? ContentType { get; set; }
    public required Stream Body { get; set; }
    public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

    public required IRequestCookieCollection Cookies { get; set; } 
    public string Method { get; set; } = HttpMethods.Get;
    public long? ContentLength { get; set; }

    public required string Host { get; set; }

    public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();


    public bool IsHttps
    {
        get
        {
            return Scheme == "https";
        }
    }

    public required string Path { get; set; }

    public required string Protocol { get; set; }

    public required string QueryString { get; set; }

    public required IQueryCollection Query { get; set; }

    public required string Scheme { get; set; }

    public string? BodyString
    {
        get
        {
            if (this.Body == null)
            {
                return null;
            }
            using var reader = new StreamReader(this.Body);
            var text = reader.ReadToEnd();
            this.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }

    protected JsonElement? json;

    public JsonElement? Json
    {
        get
        {
            if (json != null)
            {
                return json;
            }
            if (this.Body == null)
            {
                return null;
            }
            if (this.ContentType != "application/json")
            {
                return null;
            }
            using var jsonDoc = JsonDocument.Parse(Body);
            Body.Seek(0, SeekOrigin.Begin);
            json = jsonDoc.RootElement;
            return json;
        }
    }

}