
namespace ReactChat.Shared.Messaging.Config
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ExchangeType { get; set; } = RabbitMQ.Client.ExchangeType.Direct;
    }
}
