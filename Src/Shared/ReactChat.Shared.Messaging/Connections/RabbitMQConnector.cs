using RabbitMQ.Client;
using ReactChat.Shared.Messaging.Config;

namespace ReactChat.Shared.Messaging.Connections
{
    public class RabbitMQConnector
    {
        private readonly RabbitMQSettings _settings;

        public RabbitMQConnector(RabbitMQSettings settings)
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
            };

            return await factory.CreateConnectionAsync();
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            var connection = await CreateConnection();
            return await connection.CreateChannelAsync();
        }
    }
}
