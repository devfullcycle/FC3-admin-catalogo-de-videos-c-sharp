using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection 
    : ICollectionFixture<VideoRepositoryTestFixture>
{
}

public class VideoRepositoryTestFixture : BaseFixture
{
    public Video GetExampleVideo()
        =>  new (
            GetValidTitle(),
            GetValidDescription(),
            GetValidYearLaunched(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidDuration(),
            GetRandomRating()
        );


    public Rating GetRandomRating()
    {
        var enumValue = Enum.GetValues<Rating>();
        var random = new Random();
        return enumValue[random.Next(enumValue.Length)];
    }

    public string GetValidTitle()
        => Faker.Lorem.Letter(100);

    public string GetValidDescription()
        => Faker.Commerce.ProductDescription();

    public string GetTooLongDescription()
        => Faker.Lorem.Letter(4001);

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(
            new DateOnly(1960, 1, 1),
            new DateOnly(2022, 1, 1)
        ).Year;

    public int GetValidDuration()
        => new Random().Next(100, 300);

    public bool GetRandomBoolean()
        => new Random().NextDouble() < 0.5;

    public IEnumerable<CastMember> GetRandomCastMembersList()
        => Enumerable.Range(0, Random.Shared.Next(1, 5))
            .Select(_ => GetExampleCastMember());
    
    public CastMember GetExampleCastMember()
        => new CastMember(
            GetValidCastMemberName(),
            GetRandomCastMemberType()
        );

    public string GetValidCastMemberName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)new Random().Next(1, 2);

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while(categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if(categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription =
            Faker.Commerce.ProductDescription();
        if(categoryDescription.Length > 10_000)
            categoryDescription =
                categoryDescription[..10_000];
        return categoryDescription;
    }

    public Category GetValidCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription()
        );
    
    public IEnumerable<Category> GetRandomCategoriesList()
        => Enumerable.Range(0, Random.Shared.Next(1, 5))
            .Select(_ => GetValidCategory());

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public Genre GetExampleGenre() => new(GetValidGenreName(), true);

    public IEnumerable<Genre> GetRandomGenresList()
        => Enumerable.Range(0, Random.Shared.Next(1, 5))
            .Select(_ => GetExampleGenre());
}
