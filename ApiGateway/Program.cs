using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Leer configuración ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Registrar Ocelot
builder.Services.AddOcelot();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapHealthChecks("/health");

// Usar Ocelot
await app.UseOcelot();

app.Run();