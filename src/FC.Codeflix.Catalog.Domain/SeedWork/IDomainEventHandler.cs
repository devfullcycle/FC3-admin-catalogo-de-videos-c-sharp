namespace FC.Codeflix.Catalog.Domain.SeedWork;
public interface IDomainEventHandler<TDomainEvent> where TDomainEvent: DomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}
