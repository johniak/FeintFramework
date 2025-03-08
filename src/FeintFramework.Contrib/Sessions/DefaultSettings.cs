using System.Net;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Contrib.Sessions;


internal class DefaultSettings
{
    public static string sessionCookieName = "sessionid";
    public static CookieOptions sessionCookieOptions = new CookieOptions()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        MaxAge = new TimeSpan(14, 0, 0, 0),
        IsEssential = true,
        Path = "/",
        Domain = null,
    };
}