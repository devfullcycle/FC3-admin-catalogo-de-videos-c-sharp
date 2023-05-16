namespace FC.Codeflix.Catalog.Domain.SeedWork;
public abstract class DomainEvent
{
    public DateTime OccuredOn { get; set; }
    protected DomainEvent()
    {
        OccuredOn = DateTime.Now;
    }
}
