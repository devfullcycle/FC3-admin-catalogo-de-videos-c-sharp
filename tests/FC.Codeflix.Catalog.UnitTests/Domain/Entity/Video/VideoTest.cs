using System;
using FC.Codeflix.Catalog.Domain.Exceptions;
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
        var expectedTitle = _fixture.GetValidTitle();
        var expectedDescription = _fixture.GetValidDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();

        var expectedCreatedDate = DateTime.Now;
        var video = new DomainEntity.Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.Duration.Should().Be(expectedDuration);
        video.CreatedAt.Should().BeCloseTo(expectedCreatedDate, TimeSpan.FromSeconds(10));
    }
    
    [Fact(DisplayName = nameof(InstantiateThrowsExceptionWhrnNotValid))]
    [Trait("Domain", "Video - Aggregate")]
    public void InstantiateThrowsExceptionWhrnNotValid()
    {
        var expectedTitle = "";
        var expectedDescription = _fixture.GetTooLongDescription();
        var expectedYearLaunched = _fixture.GetValidYearLaunched();
        var expectedOpened = _fixture.GetRandomBoolean();
        var expectedPublished = _fixture.GetRandomBoolean();
        var expectedDuration = _fixture.GetValidDuration();

        var expectedCreatedDate = DateTime.Now;
        var action = () => new DomainEntity.Video(
            expectedTitle,
            expectedDescription,
            expectedYearLaunched,
            expectedOpened,
            expectedPublished,
            expectedDuration
        );

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Validation errors");
    }
}
