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
}
