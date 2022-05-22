using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        var genreRepository = new Repository.GenreRepository(dbContext);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext
            .Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should()
            .HaveCount(categoriesListExample.Count);
        genreCategoriesRelations.ForEach(relation => {
            var expectedCategory = categoriesListExample
                .FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach(var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        dbContext.SaveChanges();
        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(true)
        );

        var genreFromRepository = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);
        
        genreFromRepository.Should().NotBeNull();
        genreFromRepository!.Name.Should().Be(exampleGenre.Name);
        genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);
        genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromRepository.Categories.Should()
            .HaveCount(categoriesListExample.Count);
        foreach(var categoryId in genreFromRepository.Categories)
        {
            var expectedCategory = categoriesListExample
                .FirstOrDefault(x => x.Id == categoryId);
            expectedCategory.Should().NotBeNull();
        };
    }

    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetThrowWhenNotFound()
    {
        var exampleNotFoundGuid = Guid.NewGuid();
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        dbContext.SaveChanges();
        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(true)
        );

        var action = async () => await genreRepository.Get(
            exampleNotFoundGuid, 
            CancellationToken.None
        );

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleNotFoundGuid}' not found.");
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        dbContext.SaveChanges();
        var repositoryDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            repositoryDbContext
        );

        await genreRepository.Delete(
            exampleGenre, 
            CancellationToken.None
        );
        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = assertsDbContext.Genres
            .AsNoTracking().FirstOrDefault(x => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();
        var categoriesIdsList = await assertsDbContext.GenresCategories
            .AsNoTracking().Where(x => x.GenreId == exampleGenre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        categoriesIdsList.Should().HaveCount(0);
    }


    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        await genreRepository.Update(
            exampleGenre,
            CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext
            .Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should()
            .HaveCount(categoriesListExample.Count);
        genreCategoriesRelations.ForEach(relation => {
            var expectedCategory = categoriesListExample
                .FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        await genreRepository.Update(
            exampleGenre,
            CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext
            .Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should()
            .HaveCount(0);
    }

    [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var categoriesListExample = _fixture.GetExampleCategoriesList(3);
        var updateCategoriesListExample = _fixture.GetExampleCategoriesList(2);
        categoriesListExample.ForEach(
            category => exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Categories.AddRangeAsync(updateCategoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();
        exampleGenre.RemoveAllCategories();
        updateCategoriesListExample
            .ForEach(category => exampleGenre.AddCategory(category.Id));
        await genreRepository.Update(
            exampleGenre,
            CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbGenre = await assertsDbContext
            .Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        var genreCategoriesRelations = await assertsDbContext
            .GenresCategories.Where(r => r.GenreId == exampleGenre.Id)
            .ToListAsync();
        genreCategoriesRelations.Should()
            .HaveCount(updateCategoriesListExample.Count);
        genreCategoriesRelations.ForEach(relation => {
            var expectedCategory = updateCategoriesListExample
                .FirstOrDefault(x => x.Id == relation.CategoryId);
            expectedCategory.Should().NotBeNull();
        });
    }

    [Fact(DisplayName = nameof(SearchReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsItemsAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres(10);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(
            searchInput, 
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach(var resultItem in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x => x.Id == resultItem.Id);
            exampleGenre.Should().NotBeNull();
            resultItem!.Name.Should().Be(exampleGenre!.Name);
            resultItem.IsActive.Should().Be(exampleGenre.IsActive);
            resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        }
    }


    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        var actDbContext = _fixture.CreateDbContext();
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(0);
        searchResult.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres(10);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre => {
            var categoriesListToRelation = 
                _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if(categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(
                    category => exampleGenre.AddCategory(category.Id)
                );
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category
                        => new GenresCategories(category.Id, exampleGenre.Id)
                    )
                    .ToList(); ;
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        foreach (var resultItem in searchResult.Items)
        {
            var exampleGenre = exampleGenresList
                .Find(x => x.Id == resultItem.Id);
            exampleGenre.Should().NotBeNull();
            resultItem!.Name.Should().Be(exampleGenre!.Name);
            resultItem.IsActive.Should().Be(exampleGenre.IsActive);
            resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            resultItem.Categories.Should()
                .HaveCount(exampleGenre.Categories.Count);
            resultItem.Categories.Should()
                .BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenres(quantityToGenerate);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre => {
            var categoriesListToRelation =
                _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(
                    category => exampleGenre.AddCategory(category.Id)
                );
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category
                        => new GenresCategories(category.Id, exampleGenre.Id)
                    )
                    .ToList(); ;
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        foreach (var resultItem in searchResult.Items)
        {
            var exampleGenre = exampleGenresList
                .Find(x => x.Id == resultItem.Id);
            exampleGenre.Should().NotBeNull();
            resultItem!.Name.Should().Be(exampleGenre!.Name);
            resultItem.IsActive.Should().Be(exampleGenre.IsActive);
            resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            resultItem.Categories.Should()
                .HaveCount(exampleGenre.Categories.Count);
            resultItem.Categories.Should()
                .BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleListGenresByNames(
            new List<string>() {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi IA",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future"
            }
        );
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        var random = new Random();
        exampleGenresList.ForEach(exampleGenre => {
            var categoriesListToRelation =
                _fixture.GetExampleCategoriesList(random.Next(0, 4));
            if (categoriesListToRelation.Count > 0)
            {
                categoriesListToRelation.ForEach(
                    category => exampleGenre.AddCategory(category.Id)
                );
                dbContext.Categories.AddRange(categoriesListToRelation);
                var relationsToAdd = categoriesListToRelation
                    .Select(category
                        => new GenresCategories(category.Id, exampleGenre.Id)
                    )
                    .ToList(); ;
                dbContext.GenresCategories.AddRange(relationsToAdd);
            }
        });
        dbContext.SaveChanges();
        var actDbContext = _fixture.CreateDbContext(true);
        var genreRepository = new Repository.GenreRepository(
            actDbContext
        );
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var searchResult = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);
        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
        foreach (var resultItem in searchResult.Items)
        {
            var exampleGenre = exampleGenresList
                .Find(x => x.Id == resultItem.Id);
            exampleGenre.Should().NotBeNull();
            resultItem!.Name.Should().Be(exampleGenre!.Name);
            resultItem.IsActive.Should().Be(exampleGenre.IsActive);
            resultItem.CreatedAt.Should().Be(exampleGenre.CreatedAt);
            resultItem.Categories.Should()
                .HaveCount(exampleGenre.Categories.Count);
            resultItem.Categories.Should()
                .BeEquivalentTo(exampleGenre.Categories);
        }
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList =
            _fixture.GetExampleListGenres(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.GenreRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var output = await repository.Search(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneGenreListOrdered(
            exampleGenresList,
            orderBy,
            searchOrder
        );
        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);
        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var expectedItem = expectedOrderedList[indice];
            var outputItem = output.Items[indice];
            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(expectedItem!.Name);
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
