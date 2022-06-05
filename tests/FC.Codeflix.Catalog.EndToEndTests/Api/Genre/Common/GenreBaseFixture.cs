using FC.Codeflix.Catalog.EndToEndTests.Base;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenreBaseFixture
    : BaseFixture
{
    public GenrePersistence Persistence { get; set; }

    public GenreBaseFixture()
        : base()
    {
        Persistence = new GenrePersistence(CreateDbContext());
    }
}
