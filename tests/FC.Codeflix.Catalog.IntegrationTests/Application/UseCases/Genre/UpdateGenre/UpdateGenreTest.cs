using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;


namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext)
        );
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(), 
            !targetGenre.IsActive
        );

        GenreModelOutput output = await updateGenre.Handle(
            input, 
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = 
            await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
    [Trait("Integration/Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithCategoriesRelations()
    {
        List<DomainEntity.Category> exampleCategories = _fixture.GetExampleCategoriesList(10);
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
        DomainEntity.Genre targetGenre = exampleGenres[5];
        List<DomainEntity.Category> relatedCategories = exampleCategories.GetRange(0, 5);
        List<DomainEntity.Category> newRelatedCategories = exampleCategories.GetRange(5, 3);
        relatedCategories.ForEach(category => targetGenre.AddCategory(category.Id));
        List<GenresCategories> relations = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();
        await arrangeDbContext.AddRangeAsync(exampleGenres);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(relations);
        await arrangeDbContext.SaveChangesAsync();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext(true);
        UseCase.UpdateGenre updateGenre = new UseCase.UpdateGenre(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext)
        );
        UpdateGenreInput input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidGenreName(),
            !targetGenre.IsActive,
            newRelatedCategories.Select(category => category.Id).ToList()
        );

        GenreModelOutput output = await updateGenre.Handle(
            input,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive!);
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        List<Guid> relatedCategoryIdsFromOutput = 
            output.Categories.Select(relatedCategory => relatedCategory.Id).ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(input.CategoriesIds);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb =
            await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Id.Should().Be(targetGenre.Id);
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive!);
        List<Guid> relatedcategoryIdsFromDb = await assertDbContext
            .GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == input.Id)
            .Select(relation => relation.CategoryId)
            .ToListAsync();
        relatedcategoryIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
    }
}
