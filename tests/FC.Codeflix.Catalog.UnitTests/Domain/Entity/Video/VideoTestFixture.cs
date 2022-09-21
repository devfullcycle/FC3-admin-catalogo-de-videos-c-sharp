using System;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>
{}

public class VideoTestFixture : BaseFixture
{
    public DomainEntity.Video GetValidVideo() => new DomainEntity.Video(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetValidDuration()
    );

    public string GetValidTitle()
        => Faker.Lorem.Letter(100);

    public string GetValidDescription()
        => Faker.Commerce.ProductDescription();

    public int GetValidYearLaunched()
        => Faker.Date.BetweenDateOnly(
            new DateOnly(1960, 1, 1), 
            new DateOnly(2022, 1, 1)
        ).Year;

    public int GetValidDuration()
        => (new Random()).Next(100, 300);

    public string GetTooLongTitle()
        => Faker.Lorem.Letter(400);
}
