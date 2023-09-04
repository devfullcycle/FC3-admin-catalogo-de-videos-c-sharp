using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public class GenreUseCasesBaseFixture
    : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];
    
    public DomainEntity.Genre GetExampleGenre(
        bool? isActive = null,
        List<Guid>? categoriesIds = null 
    )
    {
        var genre = new DomainEntity.Genre(
            GetValidGenreName(),
            isActive ?? GetRandomBoolean()
        );
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleGenresList(
        int count = 10
    )
        => Enumerable.Range(1, count).Select(_ =>
        {
            var genre = new DomainEntity.Genre(
                GetValidGenreName(),
                GetRandomBoolean()
            );
            GetRandomIdsList()
                .ForEach(genre.AddCategory);
            return genre;
        }).ToList();        

    public List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable
            .Range(1, count ?? (new Random()).Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public Mock<IGenreRepository> GetGenreRepositoryMock()
    => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public Mock<ICategoryRepository> GetCategoryRepositoryMock()
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

    public List<DomainEntity.Category> GetExampleCategoriesList(int count = 5)
        => Enumerable.Range(0, count).Select(_ => GetExampleCategory())
            .ToList();
}
