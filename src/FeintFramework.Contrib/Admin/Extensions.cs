
using System.Reflection.Metadata;
using FeintFramework.Contrib.Admin;
using FeintFramework.Core.Config.Settings;
using FeintFramework.Core.Http;
using Microsoft.AspNetCore.Http;

namespace FeintFramework.Contrib.Admin;

public static class AdminExtensions
{
    public static string AdminUrlPrefix(this BaseSettings settings){
        if (settings.AdditionalSettings.ContainsKey(AdminConsts.SETTINGS_ADMIN_URL_PREFIX)){
            return (string)settings.AdditionalSettings[AdminConsts.SETTINGS_ADMIN_URL_PREFIX];
        }
        return DefaultSettings.adminUrlPrefix;
    }

    public static void SetAdminUrlPrefix(this BaseSettings settings, string adminUrlPrefix)
    {
        settings.AdditionalSettings[AdminConsts.SETTINGS_ADMIN_URL_PREFIX] = adminUrlPrefix;
    }

}