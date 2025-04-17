
using System.Reflection.Metadata;
using FeintFramework.Config.Settings;
using FeintFramework.Http;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Contrib.Sessions;

public static class SessionExtensions
{

    public static SessionStore Session(this FeintHttpRequest request)
    {
        if (!request.AdditionalData.ContainsKey(SessionConsts.REQUEST_SESSION_STORE_KEY))
        {
            throw new Exception("Session not found. Remember to add Session middleware.");
        }
        return (SessionStore)request.AdditionalData[SessionConsts.REQUEST_SESSION_STORE_KEY];
    }
    public static CookieOptions SessionCookieOptions(this BaseSettings settings)
    {
        if (!settings.AdditionalSettings.ContainsKey(SessionConsts.SETTINGS_SESSION_COOKIE_OPTIONS_KEY))
            return DefaultSettings.sessionCookieOptions;
        return (CookieOptions)settings.AdditionalSettings[SessionConsts.SETTINGS_SESSION_COOKIE_OPTIONS_KEY];
    }

    public static string SessionCookieName(this BaseSettings settings)
    {
        if (!settings.AdditionalSettings.ContainsKey(SessionConsts.SETTINGS_SESSION_COOKIE_NAME))
            return DefaultSettings.sessionCookieName;
        return (string)settings.AdditionalSettings[SessionConsts.SETTINGS_SESSION_COOKIE_NAME];
    }

    public static void SetSessionCookieOptions(this BaseSettings settings, CookieOptions options)
    {
        settings.AdditionalSettings[SessionConsts.SETTINGS_SESSION_COOKIE_OPTIONS_KEY] = options;
    }

    public static void SetSessionCookieName(this BaseSettings settings, string name)
    {
        settings.AdditionalSettings[SessionConsts.SETTINGS_SESSION_COOKIE_NAME] = name;
    }
}