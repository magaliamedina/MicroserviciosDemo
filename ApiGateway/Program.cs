using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Leer configuración ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Registrar Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

// Usar Ocelot
await app.UseOcelot();

app.Run();