using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection
    : ICollectionFixture<UpdateGenreTestFixture>
{ }

public class UpdateGenreTestFixture
    : GenreUseCasesBaseFixture
{
}
