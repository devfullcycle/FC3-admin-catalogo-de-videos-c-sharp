using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using Xunit;
using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[Collection(nameof(UploadMediasTestFixture))]
public class UploadMediasTest
{
    private readonly UploadMediasTestFixture _fixture;
    private readonly UseCase.UploadMedias _useCase;
    private readonly Mock<IVideoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;

    public UploadMediasTest(UploadMediasTestFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageServiceMock = new Mock<IStorageService>();
        _useCase = new UseCase.UploadMedias(
            _repositoryMock.Object, 
            _storageServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact(DisplayName = nameof(UploadMedias))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task UploadMedias()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(videoId: video.Id);
        var fileNames = new List<string>() {
            StorageFileName.Create(video.Id, nameof(video.Media), validInput.VideoFile!.Extension),
            StorageFileName.Create(video.Id, nameof(video.Trailer), validInput.TrailerFile!.Extension)
        };
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(video);
        _storageServiceMock
            .Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(validInput, CancellationToken.None);

        _repositoryMock.VerifyAll();
        _storageServiceMock.Verify(x =>
            x.Upload(
                It.Is<string>(x => fileNames.Contains(x)),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task ThrowsWhenVideoNotFound()
    {
        var video = _fixture.GetValidVideo();
        var validInput = _fixture.GetValidInput(videoId: video.Id);
        _repositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == video.Id),
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(validInput, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");
    }
}
