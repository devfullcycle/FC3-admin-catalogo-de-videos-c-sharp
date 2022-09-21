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
        "Title",
        "Description",
        2001,
        true,
        true,
        180
    );
}
