using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection 
    : ICollectionFixture<ListVideosTestFixture>
{ }

public class ListVideosTestFixture : VideoTestFixtureBase
{
    public List<DomainEntities.Video> CreateExampleVideosList() 
        => Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

    public List<DomainEntities.Video> CreateExampleVideosListWithoutRelations()
        => Enumerable.Range(1, Random.Shared.Next(2, 10))
            .Select(_ => GetValidVideo())
            .ToList();

    public (
        List<DomainEntities.Video> Videos,
        List<DomainEntities.Category> Categories,
        List<DomainEntities.Genre> Genres
    ) CreateExampleVideosListWithRelations()
    {
        var itemsQuantityToBeCreated = Random.Shared.Next(2, 10);
        List<DomainEntities.Category> categories = new();
        List<DomainEntities.Genre> genres = new();
        var videos = Enumerable.Range(1, itemsQuantityToBeCreated)
            .Select(_ => GetValidVideoWithAllProperties())
            .ToList();

        videos.ForEach(video =>
        {
            video.RemoveAllCategories();
            var qtdCategories = Random.Shared.Next(2, 5);
            for(var i = 0; i < qtdCategories; i++)
            {
                var category = GetExampleCategory();
                categories.Add(category);
                video.AddCategory(category.Id);
            }

            video.RemoveAllGenres();
            var qtdGenres = Random.Shared.Next(2, 5);
            for(var i = 0; i < qtdGenres; i++)
            {
                var genre = GetExampleGenre();
                genres.Add(genre);
                video.AddGenre(genre.Id);
            }
        });

        return (videos, categories, genres);
    }


    string GetValidCategoryName()
    {
        var categoryName = "";
        while(categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if(categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if(categoryDescription.Length > 10_000)
            categoryDescription =
                categoryDescription[..10_000];
        return categoryDescription;
    }

    DomainEntities.Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    DomainEntities.Genre GetExampleGenre() => new (
            GetValidGenreName(),
            GetRandomBoolean()
        );
}
