using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Commom;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenres;

[CollectionDefinition(nameof(ListGenresTestFixture))]
public class ListGenresTestFixtureCollection
    : ICollectionFixture<ListGenresTestFixture>
{ }

public class ListGenresTestFixture
    : GenreUseCasesBaseFixture
{

}
