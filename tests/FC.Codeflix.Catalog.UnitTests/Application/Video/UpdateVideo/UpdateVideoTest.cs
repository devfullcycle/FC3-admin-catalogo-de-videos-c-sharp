using FC.Codeflix.Catalog.Application.Interfaces;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepository;
    private readonly Mock<IUnitOfWork> _unitofWork;
    private readonly UseCase.UpdateVideo _useCase;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepository = new();
        _unitofWork = new();
        _useCase = new(_videoRepository.Object, _unitofWork.Object);
    }

    [Fact(DisplayName = nameof(UpdateVideosBasicInfo))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosBasicInfo()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video => 
                ((video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) && 
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration)))
            , It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }
}