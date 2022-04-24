using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;


namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
public abstract class CategoryUseCasesBaseFixture
    : BaseFixture
{

    public Mock<ICategoryRepository> GetRepositoryMock()
        => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
            categoryDescription =
                categoryDescription[..10_000];
        return categoryDescription;
    }

    public DomainEntity.Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
}
