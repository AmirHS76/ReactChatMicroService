using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReactChat.Shared.Messaging.Connections;
using System.Text;

namespace ReactChat.Shared.Messaging.Messaging
{
    public class RabbitMQConsumer
    {
        private readonly RabbitMQConnectionFactory _connectionFactory;

        public RabbitMQConsumer(RabbitMQConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task StartBasicConsumeAsync<T>(string queue, Func<T, BasicDeliverEventArgs, Task> onMessage, bool autoAck = false, CancellationToken cancellationToken = default)
        {
            var channel = await _connectionFactory.CreateChannelAsync();
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
                if (message != null)
                {
                    await onMessage(message, ea);
                }
                if (!autoAck)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(queue: queue, autoAck: autoAck, consumer: consumer);
        }
    }
} 