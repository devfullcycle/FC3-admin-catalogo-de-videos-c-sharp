namespace FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
public interface ISearchableRepository<Taggregate>
    where Taggregate : AggregateRoot
{
    Task<SearchOutput<Taggregate>> Search(
        SearchInput input,
        CancellationToken cancellationToken
    );
}
