using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection 
    : ICollectionFixture<ListVideosTestFixture>
{ }

public class ListVideosTestFixture : VideoTestFixtureBase
{
}
