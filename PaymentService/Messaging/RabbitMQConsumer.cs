using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PaymentService.Models;

namespace PaymentService.Messaging
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly ConnectionFactory _factory;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger)
        {
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IConnection connection = null;
            IChannel channel = null;

            // 🔁 Retry hasta conectar
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("🔌 Intentando conectar a RabbitMQ...");

                    connection =
                        await _factory.CreateConnectionAsync();

                    channel =
                        await connection.CreateChannelAsync();

                    _logger.LogInformation("✅ Conectado a RabbitMQ");

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "⚠️ RabbitMQ no disponible. Reintentando en 5 segundos..."
                    );

                    await Task.Delay(5000, stoppingToken);
                }
            }

            await channel.QueueDeclareAsync(
                queue: "order_created_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer =
                new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();

                var message =
                    Encoding.UTF8.GetString(body);

                var orderEvent =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                _logger.LogInformation(
                    "💳 Pago recibido OrderId {OrderId} Producto {Product} Total {Total}",
                    orderEvent.OrderId,
                    orderEvent.Product,
                    orderEvent.Total
                );

                await Task.Delay(1000);

                _logger.LogInformation(
                    "✅ Pago procesado OrderId {OrderId}",
                    orderEvent.OrderId
                );
            };

            await channel.BasicConsumeAsync(
                queue: "order_created_queue",
                autoAck: true,
                consumer: consumer
            );

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}