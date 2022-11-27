using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
{ }

public class UpdateVideoTestFixture : VideoTestFixtureBase
{
    public UseCase.UpdateVideoInput CreateValidInput(Guid videoId) 
        => new(
            videoId,
            GetValidTitle(),
            GetValidDescription(),
            GetValidYearLaunched(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidDuration(),
            GetRandomRating()
        );
}
