using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System;
using System.Collections.Generic;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection 
    : ICollectionFixture<CreateVideoTestFixture>
{ }

public class CreateVideoTestFixture : VideoTestFixtureBase
{
    internal CreateVideoInput CreateValidCreateVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null
    ) => new(
        GetValidTitle(),
        GetValidDescription(),
        GetValidYearLaunched(),
        GetRandomBoolean(),
        GetRandomBoolean(),
        GetValidDuration(),
        GetRandomRating(),
        categoriesIds,
        genresIds,
        castMembersIds,
        thumb,
        banner,
        thumbHalf
    );
}
