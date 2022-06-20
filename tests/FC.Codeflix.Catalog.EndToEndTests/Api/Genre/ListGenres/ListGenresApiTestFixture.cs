using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenres;

[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCiollection
    : ICollectionFixture<ListGenresApiTestFixture>
{ }

public class ListGenresApiTestFixture
    : GenreBaseFixture
{
}
