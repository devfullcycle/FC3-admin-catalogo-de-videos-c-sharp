using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.SeedWork;
public class AggregateRootTest
{
    [Fact(DisplayName = nameof(RaiseEvent))]
    [Trait("Domain", "AggregateRoot")]
    public void RaiseEvent()
    {
        var domainEvent = new DomainEventFake();
        var aggregate = new AggregateRootFake();

        aggregate.RaiseEvent(domainEvent);

        aggregate.Events.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ClearEvents))]
    [Trait("Domain", "AggregateRoot")]
    public void ClearEvents()
    {
        var domainEvent = new DomainEventFake();
        var aggregate = new AggregateRootFake();
        aggregate.RaiseEvent(domainEvent);

        aggregate.ClearEvents();

        aggregate.Events.Should().BeEmpty();
    }
}
