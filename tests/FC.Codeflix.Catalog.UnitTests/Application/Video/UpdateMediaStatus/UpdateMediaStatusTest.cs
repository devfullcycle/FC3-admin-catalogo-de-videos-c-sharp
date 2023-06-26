using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateMediaStatus;

[Collection(nameof(UpdateMediaStatusTestFixture))]
public class UpdateMediaStatusTest
{
    private readonly Mock<IVideoRepository> _videoRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UseCase.UpdateMediaStatus _useCase;
    private readonly UpdateMediaStatusTestFixture _fixture;

    public UpdateMediaStatusTest(UpdateMediaStatusTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepository = new Mock<IVideoRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _useCase = new UseCase.UpdateMediaStatus(
            _videoRepository.Object,
            _unitOfWork.Object,
            Mock.Of<ILogger<UseCase.UpdateMediaStatus>>());
    }

    [Fact(DisplayName = nameof(HandleWhenSucceededEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenSucceededEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetSuccededEncodingInput(exampleVideo.Id);
        _videoRepository.Setup(x => x.Get(
            exampleVideo.Id,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Published.Should().Be(exampleVideo.Published);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Completed);
        exampleVideo.Media!.EncodedPath.Should().Be(input.EncodedPath);
        _videoRepository.VerifyAll();
        _videoRepository.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(HandleWhenFailedEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async Task HandleWhenFailedEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetFailedEncodingInput(exampleVideo.Id);
        _videoRepository.Setup(x => x.Get(
            exampleVideo.Id,
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Published.Should().Be(exampleVideo.Published);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        exampleVideo.Media!.Status.Should().Be(MediaStatus.Error);
        exampleVideo.Media!.EncodedPath.Should().BeNull();
        _videoRepository.VerifyAll();
        _videoRepository.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}
