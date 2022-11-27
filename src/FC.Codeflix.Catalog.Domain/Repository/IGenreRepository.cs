using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Domain.Repository;
public interface IGenreRepository
    : IGenericRepository<Genre>,
    ISearchableRepository<Genre>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );

    public Task<IReadOnlyList<Genre>> GetListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );
}
