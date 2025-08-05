using Newtonsoft.Json;
using RabbitMQ.Client;
using ReactChat.Shared.Messaging.Connections;
using System.Text;

namespace ReactChat.Shared.Messaging.Messaging
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQConnector _connectionFactory;

        public RabbitMQPublisher(RabbitMQConnector connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default)
        {
            var channel = await _connectionFactory.CreateChannelAsync();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                body: body,
                cancellationToken: cancellationToken
            );
            await channel.CloseAsync(cancellationToken: cancellationToken);
            await channel.DisposeAsync();
        }
    }
}
