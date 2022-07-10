using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Domain.Repository;
public interface ICastMemberRepository 
    : IGenericRepository<CastMember>, 
    ISearchableRepository<CastMember>
{
}