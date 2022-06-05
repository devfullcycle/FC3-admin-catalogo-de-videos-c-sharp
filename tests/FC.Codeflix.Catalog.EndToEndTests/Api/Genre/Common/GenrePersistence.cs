using FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenrePersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public GenrePersistence(CodeflixCatalogDbContext context)
        => _context = context;
}
