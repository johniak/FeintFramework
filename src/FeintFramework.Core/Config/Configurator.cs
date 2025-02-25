

using FeintFramework.Core.Config.Settings;
using FeintFramework.Core.Http;
using FeintFramework.Core.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace FeintFramework.Core.Config;
public static class Configurator
{

    public static BaseSettings Settings;

    private static void Validate()
    {
        if (Settings == null)
        {
            throw new Exception("Settings not configured");
        }
        if (Settings.RootUrlPatterns == null)
        {
            throw new Exception("RootUrlPatterns not configured");
        }
    }

    public static void Configure(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AllowSynchronousIO = true;
        });

        var app = builder.Build();
        var mainUrlPatterns = Settings.RootUrlPatterns;
        var router = new Router(mainUrlPatterns);
        var serverHanler = new KestrelServerHandler(router.HandleRequest);
        app.Use(async (HttpContext context, RequestDelegate next) =>
        {
            serverHanler.HandleRequest(context);
            await context.Response.CompleteAsync();
        });
        app.Run("http://0.0.0.0:9000");
    }

}