using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection
    : ICollectionFixture<UpdateGenreTestFixture>
{ }

public class UpdateGenreTestFixture
    : GenreUseCasesBaseFixture
{
}
