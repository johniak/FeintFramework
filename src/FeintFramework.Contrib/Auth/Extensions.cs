using System.Reflection;
using FeintFramework.Contrib.Auth.Backends;
using FeintFramework.Contrib.Auth.Models;
using FeintFramework.Core.Apps;
using FeintFramework.Core.Config;
using FeintFramework.Core.Config.Settings;
using FeintFramework.Core.Http;
using FeintFramework.Db;
using LinqToDB;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Contrib.Auth;


public static class AuthExtensions
{
    public static T User<T>(this FeintHttpRequest request) where T : IUser, new()
    {
        var user = request.AdditionalData[AuthConsts.REQUEST_USER_STORE_KEY];
        if (user == null)
        {
            throw new Exception("User not Authenticated, please use User without generic type to get user or AnonymusUser to get default user");
        }
        return (T)user!;
    }

    public static IUser User(this FeintHttpRequest request)
    {
        var user = request.AdditionalData[AuthConsts.REQUEST_USER_STORE_KEY];
        if (user == null)
        {
            return new AnonymusUser();
        }
        return (IUser)user!;
    }

    

    public static Type GetUserType(this BaseSettings settings, IUser user)
    {
        return (Type)settings.AdditionalSettings[AuthConsts.REQUEST_USER_STORE_KEY];
    }

    public static void SetUserType(this BaseSettings settings, Type userType)
    {
        settings.AdditionalSettings[AuthConsts.REQUEST_USER_STORE_KEY] = userType;
    }

    public static dynamic GetUsersTable(this BaseSettings settings) 
    {
        if (!settings.AdditionalSettings.ContainsKey(AuthConsts.SETTINGS_USER_MANAGER))
        {
            return Connections.Connection!.GetTable<User>();
        }
        return settings.AdditionalSettings[AuthConsts.SETTINGS_USER_MANAGER];
    }

    public static void SetUsersTable<T>(this BaseSettings settings, ITable<T> table)
    {
        settings.AdditionalSettings[AuthConsts.SETTINGS_USER_MANAGER] = table;
    }
    

    public static string GetUserModelName(this BaseSettings settings)
    {
        if (!settings.AdditionalSettings.ContainsKey(AuthConsts.SETTINGS_USER_TYPE))
        {
            return "Auth.User";
        }
        var userType = (Type)settings.AdditionalSettings[AuthConsts.SETTINGS_USER_TYPE];

        return (string)FindClosestSubclassByNamespace(userType, typeof(BaseApplication)).Name!;
    }

    public static void SetAuthBackend<T>(this BaseSettings settings, T backend) where T : BaseBackend
    {
        settings.AdditionalSettings[AuthConsts.SETTINGS_AUTH_BACKEND] = backend;
    }

    public static BaseBackend AuthBackend(this BaseSettings settings,FeintHttpRequest request)
    {
        if (!settings.AdditionalSettings.ContainsKey(AuthConsts.SETTINGS_AUTH_BACKEND))
        {
            return new UserModelBackend(request);
        }
        var authBackendType = (Type)settings.AdditionalSettings[AuthConsts.SETTINGS_AUTH_BACKEND];
        return (BaseBackend)Activator.CreateInstance(authBackendType, request)!;
    }

    public static Type FindClosestSubclassByNamespace(Type startType, Type baseType)
    {
        Assembly assembly = baseType.Assembly;
        string startNamespace = startType.Namespace ?? "";
        var startSegments = startNamespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        Type bestCandidate = null;
        int bestCommonSegments = -1;
        int bestTotalSegments = int.MaxValue;

        foreach (var candidate in assembly.GetTypes())
        {
            if (candidate == baseType || !baseType.IsAssignableFrom(candidate))
                continue;

            if (string.IsNullOrEmpty(candidate.Namespace))
                continue;

            var candidateSegments = candidate.Namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            int common = GetCommonPrefixLength(startSegments, candidateSegments);

            if (common > bestCommonSegments || (common == bestCommonSegments && candidateSegments.Length < bestTotalSegments))
            {
                bestCommonSegments = common;
                bestTotalSegments = candidateSegments.Length;
                bestCandidate = candidate;
            }
        }

        return bestCandidate;
    }
    private static int GetCommonPrefixLength(string[] seg1, string[] seg2)
    {
        int len = Math.Min(seg1.Length, seg2.Length);
        int count = 0;
        for (int i = 0; i < len; i++)
        {
            if (seg1[i] == seg2[i])
                count++;
            else
                break;
        }
        return count;
    }
}