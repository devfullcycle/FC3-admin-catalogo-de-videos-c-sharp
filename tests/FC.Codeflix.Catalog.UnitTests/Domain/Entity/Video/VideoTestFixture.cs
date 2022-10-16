using Xunit;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture>
{}

public class VideoTestFixture : VideoTestFixtureBase
{ }
