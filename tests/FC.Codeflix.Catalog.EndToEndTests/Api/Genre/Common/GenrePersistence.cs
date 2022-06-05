using FC.Codeflix.Catalog.Infra.Data.EF;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

using System.Threading.Tasks;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenrePersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public GenrePersistence(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task InsertList(List<DomainEntity.Genre> genres)
    {
        await _context.AddRangeAsync(genres);
        await _context.SaveChangesAsync();
    }
}
