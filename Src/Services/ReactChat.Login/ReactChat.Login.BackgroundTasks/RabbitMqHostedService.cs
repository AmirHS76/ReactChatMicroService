using Microsoft.Extensions.Hosting;
using ReactChat.Login.Application.DTOs;
using ReactChat.Shared.Messaging.Messaging;

namespace ReactChat.Login.BackgroundTasks
{
    public class RabbitMqHostedService(RabbitMQConsumer consumer) : BackgroundService
    {
        private readonly RabbitMQConsumer _consumer = consumer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.StartBasicConsumeAsync<NewUserDTO>(
            queue: RabbitMQQueues.UserRegistrationQueue,
            onMessage: async (message, ea) =>
            {
                Console.WriteLine($"Received message: {message.Email}");
                await Task.Delay(1000);
            },
            autoAck: false,
            cancellationToken: stoppingToken
        );
        }
    }
}
