using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReactChat.Shared.Messaging.Connections;
using System.Text;

namespace ReactChat.Shared.Messaging.Messaging
{
    public class RabbitMQConsumer
    {
        private readonly RabbitMQConnector _connectionFactory;

        public RabbitMQConsumer(RabbitMQConnector connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task StartBasicConsumeAsync<T>(string queue, Func<T, BasicDeliverEventArgs, Task> onMessage, bool autoAck, CancellationToken cancellationToken)
        {
            var channel = await _connectionFactory.CreateChannelAsync();
            var consumer = new AsyncEventingBasicConsumer(channel);
            await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false,
                arguments: null);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
                if (message != null)
                {
                    await onMessage(message, ea);
                }
                if (!autoAck)
                {
                    await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                }
            };

            await channel.BasicConsumeAsync(queue: queue, autoAck: autoAck, consumer: consumer, cancellationToken);
        }
    }
}