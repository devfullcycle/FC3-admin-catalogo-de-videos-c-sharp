using Moq;
using Xunit;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;
using FC.Codeflix.Catalog.Application.Interfaces;
using System.Threading;
using System;
using FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideo()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object, 
            unitOfWorkMock.Object
        );
        var input = _fixture.CreateValidCreateVideoInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(
            It.Is<DomainEntities.Video>(
                video =>
                    video.Title == input.Title &&
                    video.Published == input.Published &&
                    video.Description == input.Description &&
                    video.Duration == input.Duration &&
                    video.Rating == input.Rating &&
                    video.Id != Guid.Empty &&
                    video.YearLaunched == input.YearLaunched &&
                    video.Opened == input.Opened
            ), 
            It.IsAny<CancellationToken>())
        );
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Use Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(
        CreateVideoInput input,
        string expectedValidationError)
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage($"There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should()
            .Be(expectedValidationError);    
        repositoryMock.Verify(
            x => x.Insert(It.IsAny<DomainEntities.Video>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
