using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection 
    : ICollectionFixture<CreateVideoTestFixture>
{ }

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    internal CreateVideoInput CreateValidCreateVideoInput() => new(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetValidDuration(),
        GetRandomRating());
}
