using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTestFixtureCollection :
    ICollectionFixture<GetGenreApiTestFixture>
{ }

public class GetGenreApiTestFixture
    : GenreBaseFixture
{
    public GetGenreApiTestFixture()
        : base()
    {

    }
}
