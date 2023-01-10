using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Video> _videos => _context.Set<Video>();

    public VideoRepository(CodeflixCatalogDbContext context)
        => _context = context;


    public async Task Insert(Video aggregate, CancellationToken cancellationToken) 
        => await _videos.AddAsync(aggregate, cancellationToken);

    public Task Delete(Video aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Video> Get(Guid id, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task Update(Video aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();
}
