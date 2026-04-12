using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrderService.Models;

namespace OrderService.Messaging
{
    public class RabbitMQPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQPublisher()
        {
            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq"
            };
        }

        public async Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent)
        {
            await using var connection =
                await _factory.CreateConnectionAsync();

            await using var channel =
                await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "order_created_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(orderEvent);
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "order_created_queue",
                body: body
            );
        }
    }
}