using FC.Codeflix.Catalog.Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.Application;
public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider;
    public DomainEventPublisher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task PublishAsync<TDomainEvent>(
        TDomainEvent domainEvent, CancellationToken cancellationToken)
            where TDomainEvent : DomainEvent
    {
        var handlers = _serviceProvider
            .GetServices<IDomainEventHandler<TDomainEvent>>();
        if (handlers is null || !handlers.Any()) return;
        foreach (var handler in handlers)
            await handler.HandleAsync(domainEvent, cancellationToken);
    }
}
