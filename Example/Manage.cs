using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FeintFramework.Core.Routing;

using Example;
using FeintFramework.Core.Http; // Zakładamy, że masz swój system routingu

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var mainUrlPatterns = new MainUrlPatterns();
var router = new Router(mainUrlPatterns);
var serverHanler = new KestrelServerHandler(router.HandleRequest);
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    serverHanler.HandleRequest(context);
    await context.Response.CompleteAsync();
});
