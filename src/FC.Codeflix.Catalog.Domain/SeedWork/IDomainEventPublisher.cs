namespace FC.Codeflix.Catalog.Domain.SeedWork;
public interface IDomainEventPublisher
{
    Task PublishAsync(DomainEvent domainEvent);
}
