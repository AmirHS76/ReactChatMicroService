using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace ReactChat.Login.Infrastructure.Messaging
{
    public class RabbitMQConsumer
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private IConnection _connection = null!;
        private IChannel _channel = null!;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger)
        {
            _logger = logger;
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            await CreateConnections(cancellationToken);

            var consumer = CreateConsumer(cancellationToken);

            await _channel.BasicConsumeAsync(queue: "NewUser", autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        public async Task CreateConnections(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "admin",
                Password = "admin",
                AutomaticRecoveryEnabled = true,
                ConsumerDispatchConcurrency = 5
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: "NewUser", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
        }

        public AsyncEventingBasicConsumer CreateConsumer(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Received message: {message}");

                try
                {
                    await Task.Delay(200, cancellationToken);
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed processing message.");
                }
            };
            return consumer;
        }
    }
}
