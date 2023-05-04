using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Application.EventHandlers;
public class SendToEncoderEventHandler : IDomainEventHandler<VideoUploadedEvent>
{
    private readonly IMessageProducer _messageProducer;

    public SendToEncoderEventHandler(IMessageProducer messageProducer)
        => _messageProducer = messageProducer;

    public Task HandleAsync(VideoUploadedEvent domainEvent, CancellationToken cancellationToken)
        => _messageProducer.SendMessageAsync(domainEvent, cancellationToken);
}
