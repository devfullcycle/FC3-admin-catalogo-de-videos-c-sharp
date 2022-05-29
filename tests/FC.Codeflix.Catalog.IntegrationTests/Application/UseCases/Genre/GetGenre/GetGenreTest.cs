using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using System.Threading.Tasks;
using Xunit;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using System.Threading;
using FluentAssertions;
using System;
using FC.Codeflix.Catalog.Application.Exceptions;
using System.Linq;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var expectedGenre = genresExampleList[5];
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreThrowsWhenNotFound()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var randomGuid = Guid.NewGuid();
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(randomGuid);

        var action = async () 
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
    [Trait("Integration/Application", "GetGenre - Use Cases")]
    public async Task GetGenreWithCategoryRelations()
    {
        var genresExampleList = _fixture.GetExampleListGenres(10);
        var categoriesExampleList = _fixture.GetExampleCategoriesList(5);
        var expectedGenre = genresExampleList[5];
        categoriesExampleList.ForEach(
            category => expectedGenre.AddCategory(category.Id)
        );
        var dbArrangeContext = _fixture.CreateDbContext();
        await dbArrangeContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbArrangeContext.Genres.AddRangeAsync(genresExampleList);
        await dbArrangeContext.GenresCategories.AddRangeAsync(
            expectedGenre.Categories.Select(
                categoryId => new GenresCategories(
                    categoryId, 
                    expectedGenre.Id
                )
            )
        );
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.GetGenre(genreRepository);
        var input = new UseCase.GetGenreInput(expectedGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);
        output.Categories.ToList().ForEach(
            relationModel =>
            {
                expectedGenre.Categories.Should().Contain(relationModel.Id);
                relationModel.Name.Should().BeNull();
            }
        );
    }
}
