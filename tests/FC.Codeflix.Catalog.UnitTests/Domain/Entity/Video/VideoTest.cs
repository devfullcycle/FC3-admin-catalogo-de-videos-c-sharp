using System;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private readonly VideoTestFixture _fixture;

    public VideoTest(VideoTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video - Aggregate")]
    public void Instantiate()
    {
        var expectedCreatedDate = DateTime.Now;
        var video = new DomainEntity.Video(
            "Title",
            "Description",
            2001,
            true,
            true,
            180
        );


        video.Title.Should().Be("Title");
        video.Description.Should().Be("Description");
        video.Opened.Should().Be(true);
        video.Published.Should().Be(true);
        video.YearLaunched.Should().Be(2001);
        video.Duration.Should().Be(180);
        video.CreatedAt.Should().BeCloseTo(expectedCreatedDate, TimeSpan.FromSeconds(10));
    }
}
