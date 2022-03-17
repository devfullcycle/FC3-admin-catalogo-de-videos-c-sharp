
using Microsoft.EntityFrameworkCore;
using System.Threading;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using System;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTest
{
    private readonly CategoryRepositoryTestFixture _fixture;

    public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var dbCategory = await categoryRepository.Get(
            exampleCategory.Id, 
            CancellationToken.None);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory.Id.Should().Be(exampleCategory.Id);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetThrowIfNotFound()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var task = async () => await categoryRepository.Get(
            exampleId,
            CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Catgory '{exampleId}' not found.");
    }


    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var newCategoryValues = _fixture.GetExampleCategory();
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(15);
        exampleCategoriesList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);
        await categoryRepository.Update(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(exampleCategory.Name);
        dbCategory.Id.Should().Be(exampleCategory.Id);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }
}
