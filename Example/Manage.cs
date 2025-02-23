using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using FeintFramework.Core.Routing;

using Example;
using FeintFramework.Core.Http;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
});

var app = builder.Build();
var mainUrlPatterns = new MainUrlPatterns();
var router = new Router(mainUrlPatterns);
var serverHanler = new KestrelServerHandler(router.HandleRequest);
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    serverHanler.HandleRequest(context);
    await context.Response.CompleteAsync();
});
app.Run("http://0.0.0.0:8080");