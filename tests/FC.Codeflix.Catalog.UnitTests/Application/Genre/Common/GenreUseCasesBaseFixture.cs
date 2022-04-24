using FC.Codeflix.Catalog.UnitTests.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCasesBaseFixture
    : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];
}
