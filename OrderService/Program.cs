using Polly;
using Polly.Extensions.Http;
var builder = WebApplication.CreateBuilder(args);

// Registrar controladores
builder.Services.AddControllers();

builder.Services
.AddHttpClient<OrderService.Services.PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://paymentservice:8080");
})
.AddPolicyHandler(GetTimeoutPolicy())        // ⏱ primero timeout
.AddPolicyHandler(GetRetryPolicy())          // 🔁 luego retry
.AddPolicyHandler(GetCircuitBreakerPolicy()); // 🔴 luego circuit

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapear controladores
app.MapControllers();

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