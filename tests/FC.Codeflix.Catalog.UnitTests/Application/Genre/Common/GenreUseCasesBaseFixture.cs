using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCasesBaseFixture
    : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];
    public DomainEntity.Genre GetExampleGenre()
        => new(
            GetValidGenreName(), 
            GetRandomBoolean()
        );

    public Mock<IGenreRepository> GetGenreRepositoryMock()
    => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public Mock<ICategoryRepository> GetCategoryRepositoryMock()
        => new();
}
