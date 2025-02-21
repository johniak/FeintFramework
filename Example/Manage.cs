using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FeintFramework.Routing; // Zakładamy, że masz swój system routingu

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Definiujemy middleware, który przechwytuje wszystkie żądania
app.Use(async (context, next) =>
{
    // Tutaj możesz przekazać żądanie do swojego systemu routingu
    // Przykładowo, wywołanie metody obsługującej wszystkie zapytania:
    await CustomRoutingHandler(context);
});
