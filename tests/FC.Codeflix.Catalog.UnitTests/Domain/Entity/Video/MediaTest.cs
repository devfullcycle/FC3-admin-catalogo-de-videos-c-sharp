using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentAssertions;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class MediaTest
{
    private readonly VideoTestFixture _fixture;

    public MediaTest(VideoTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Media - Entities")]
    public void Instantiate()
    {
        var expectedFilePath = _fixture.GetValidMediaPath();

        var media = new Media(expectedFilePath);

        media.Id.Should().NotBe(default(Guid));
        media.FilePath.Should().Be(expectedFilePath);
        media.Status.Should().Be(MediaStatus.Pending);
    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsSentToEncode()
    {
        var media = _fixture.GetValidMedia();

        media.UpdateAsSentToEncode();

        media.Status.Should().Be(MediaStatus.Processing);
    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsEncoded()
    {
        var media = _fixture.GetValidMedia();
        var encodedExamplePath = _fixture.GetValidMediaPath();
        media.UpdateAsSentToEncode();

        media.UpdateAsEncoded(encodedExamplePath);

        media.Status.Should().Be(MediaStatus.Completed);
        media.EncodedPath.Should().Be(encodedExamplePath);
    }
}
