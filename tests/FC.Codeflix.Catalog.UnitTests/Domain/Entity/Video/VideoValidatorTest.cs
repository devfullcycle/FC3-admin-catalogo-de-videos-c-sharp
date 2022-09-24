using System.Linq;
using FC.Codeflix.Catalog.Domain.Validation;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validator;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[Collection(nameof(VideoTestFixture))]
public class VideoValidatorTest
{
    private readonly VideoTestFixture _fixture;

    public VideoValidatorTest(VideoTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsValidWhenVideoIsValid()
    {
        var validVideo = _fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeFalse();
        notificationValidationHandler.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsTooLong()
    {
        var invalidVideo = new DomainEntity.Video(
            _fixture.GetTooLongTitle(),
            _fixture.GetValidDescription(),
            _fixture.GetValidYearLaunched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.ToList().First()
            .Message.Should().Be("'Title' should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsEmpty()
    {
        var invalidVideo = new DomainEntity.Video(
            "",
            _fixture.GetValidDescription(),
            _fixture.GetValidYearLaunched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.ToList().First()
            .Message.Should().Be("'Title' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsEmpty))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsEmpty()
    {
        var invalidVideo = new DomainEntity.Video(
            _fixture.GetValidTitle(),
            "",
            _fixture.GetValidYearLaunched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.ToList().First()
            .Message.Should().Be("'Description' is required");
    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenDescriptionIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenDescriptionIsTooLong()
    {
        var invalidVideo = new DomainEntity.Video(
            _fixture.GetValidTitle(),
            _fixture.GetTooLongDescription(),
            _fixture.GetValidYearLaunched(),
            _fixture.GetRandomBoolean(),
            _fixture.GetRandomBoolean(),
            _fixture.GetValidDuration(),
            _fixture.GetRandomRating()
        );
        var notificationValidationHandler = new NotificationValidationHandler();
        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);

        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.ToList().First()
            .Message.Should().Be("'Description' should be less or equal 4000 characters long");
    }
}
