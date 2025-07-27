using Newtonsoft.Json;
using RabbitMQ.Client;
using ReactChat.Shared.Messaging.Connections;
using System.Text;

namespace ReactChat.Shared.Messaging.Messaging
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQConnectionFactory _connectionFactory;

        public RabbitMQPublisher(RabbitMQConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task PublishAsync<T>(T message, string exchange, string routingKey, IBasicProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var channel = await _connectionFactory.CreateChannelAsync();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );

            channel.Close();
            channel.Dispose();
        }
    }
}
