namespace ReactChat.Login.Application.Interfaces
{
    public interface IMessageConsumer
    {
        void Consume(CancellationToken cancellationToken);
    }
}
