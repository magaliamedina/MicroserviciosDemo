using Polly;
using Polly.Extensions.Http;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Registrar controladores
builder.Services.AddControllers();

builder.Services
.AddHttpClient<OrderService.Services.PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://paymentservice:8080");
})
.AddPolicyHandler(GetFallbackPolicy())       // 🟡 primero fallback
.AddPolicyHandler(GetTimeoutPolicy())        // ⏱ primero timeout
.AddPolicyHandler(GetRetryPolicy())          // 🔁 luego retry
.AddPolicyHandler(GetCircuitBreakerPolicy()); // 🔴 luego circuit

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapear controladores
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),

            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine(
                    $"🔁 Retry {retryAttempt} después de {timespan.TotalSeconds} segundos"
                );
            }
        );
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            2,
            TimeSpan.FromSeconds(30),

            onBreak: (result, breakDelay) =>
            {
                Console.WriteLine("🔴 Circuito ABIERTO");
            },

            onReset: () =>
            {
                Console.WriteLine("🟢 Circuito RESETEADO");
            }
        );
}

static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
{
    return Policy.TimeoutAsync<HttpResponseMessage>(
        TimeSpan.FromSeconds(5)
    );
}

static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
{
    return Policy<HttpResponseMessage>
        .Handle<Exception>()
        .FallbackAsync(
            fallbackAction: async (cancellationToken) =>
            {
                Console.WriteLine("🟡 Fallback ejecutado");

                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        "{\"status\":\"failed\",\"message\":\"Payment service no disponible\"}",
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };

                return await Task.FromResult(response);
            }
        );
}