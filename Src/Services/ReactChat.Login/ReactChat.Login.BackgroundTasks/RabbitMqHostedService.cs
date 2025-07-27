using Microsoft.Extensions.Hosting;
using ReactChat.Login.Infrastructure.Messaging;

namespace ReactChat.Login.BackgroundTasks
{
    public class RabbitMqHostedService(RabbitMQConsumer consumer) : BackgroundService
    {
        private readonly RabbitMQConsumer _consumer = consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => _consumer.ConsumeAsync(stoppingToken);
    }
}
