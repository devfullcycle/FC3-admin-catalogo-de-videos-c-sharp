namespace FC.Codeflix.Catalog.Application.Interfaces;
public interface IMessageProducer
{
    Task SendMessageAsync<T>(T message, CancellationToken cancellationToken);
}
