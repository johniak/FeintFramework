using System.Collections;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Http;

public class FeintFrameworkResponseCookies : IResponseCookies, IEnumerable<(string Key, string Value, CookieOptions Options)>
{
    protected CookieOptions defaultOptions = new CookieOptions();
    protected Dictionary<(string Name, string domain, string path), (string Value, CookieOptions Options)> Cookies = new Dictionary<(string Name, string domain, string path), (string Value, CookieOptions Options)>();
    public void Append(string key, string value)
    {
        Append(key, value, defaultOptions);
    }

    public void Append(string key, string value, CookieOptions options)
    {
        var domain = options.Domain ?? "";
        var path = options.Path ?? "";
        Cookies.Add((key, domain, path), (value, options));
    }

    public void Delete(string key)
    {
        Delete(key, defaultOptions);
    }

    public void Delete(string key, CookieOptions options)
    {
        var domain = options.Domain ?? "";
        var path = options.Path ?? "";
        Cookies.Remove((key, domain, path));
    }

    public IEnumerator<(string Key, string Value, CookieOptions Options)> GetEnumerator()
    {
        foreach (var cookie in Cookies)
        {
            yield return (cookie.Key.Name, cookie.Value.Value, cookie.Value.Options);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}