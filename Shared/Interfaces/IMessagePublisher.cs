namespace Shared.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message);
    }
}
