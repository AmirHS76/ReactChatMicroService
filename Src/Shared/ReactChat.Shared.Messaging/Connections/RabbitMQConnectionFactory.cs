using RabbitMQ.Client;
using ReactChat.Shared.Messaging.Config;

namespace ReactChat.Shared.Messaging.Connections
{
    public class RabbitMQConnectionFactory
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQConnectionFactory(RabbitMQSettings settings)
        {
            _settings = settings;
        }

        public async Task<IConnection> CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            return await factory.CreateConnectionAsync();
        }

        public async Task<IModel> CreateChannelAsync()
        {
            var connection = await CreateConnection();
            return connection.CreateModel();
        }
    }
}
