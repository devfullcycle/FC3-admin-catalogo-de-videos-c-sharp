using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection
    : ICollectionFixture<CreateGenreApiTestFixture>
{ }

public class CreateGenreApiTestFixture
    : GenreBaseFixture
{
}
