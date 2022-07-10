using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class CastMemberRepository : ICastMemberRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<CastMember> _castMembers => _context.Set<CastMember>();

    public CastMemberRepository(CodeflixCatalogDbContext context) 
        => _context = context;

    public async Task Insert(CastMember aggregate, CancellationToken cancellationToken) 
        => await _castMembers.AddAsync(aggregate, cancellationToken);

    public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(CastMember aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
