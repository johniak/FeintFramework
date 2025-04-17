

using FeintFramework.Config.Settings;
using FeintFramework.Http;
using FeintFramework.Routing;
using FeintFramework.Templating;
using FeintFramework.Templating.Tags;
using FeintFramework.Db;
using FeintFramework.Db.Migrator;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace FeintFramework.Config;
public static class Configurator
{

    public static BaseSettings Settings;

    public static void Migrate()
    {
        var runner = new MigrationRunner(Settings.DatabaseHandler, Settings.InstalledApps);
        runner.RunMigrations();
    }

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
        TagRegistry.Register("if", IfTag.ParseIfTag);
        TagRegistry.Register("for", ForTag.ParseForTag);
        TagRegistry.Register("extends", ExtendsTag.ParseExtendsTag);
        TagRegistry.Register("block", BlockTag.ParseBlockTag);
        TagRegistry.Register("include", IncludeTag.ParseIncludeTag);
        TagRegistry.Register("url", UrlTag.ParseUrlTag);
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
            using (var db = new DataConnection(Settings.DatabaseProvider, Settings.DatabaseConnectionString))
            {
                Connections.Connection = db;
                try
                {
                    serverHanler.HandleRequest(context);
                    await context.Response.CompleteAsync();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Connections.Connection = null;
                }
            }
        });
        app.Run("http://0.0.0.0:9000");
    }

}