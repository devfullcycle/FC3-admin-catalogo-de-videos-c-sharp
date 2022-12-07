using Moq;
using Xunit;
using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.Interfaces;
using System.Threading;
using System;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using System.Linq;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using System.Collections.Generic;
using FC.Codeflix.Catalog.Application.Exceptions;
using System.IO;
using FC.Codeflix.Catalog.Domain.Extensions;

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
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput();

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
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithThumb()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbName = "thumb.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInput(
            thumb: _fixture.GetValidImageFileInput());

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
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithBanner()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbHalfName = "thumbhalf.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedThumbHalfName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInput(
            thumbHalf: _fixture.GetValidImageFileInput());

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
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithThumbHalf()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedBannerName = "banner.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedBannerName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInput(
            banner: _fixture.GetValidImageFileInput());

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
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(expectedBannerName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithAllImages()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedThumbHalfName = "thumbhalf.jpg";
        var expectedThumbName = "thumb.jpg";
        var expectedBannerName = "banner.jpg";
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedBannerName);
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith("-thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedThumbHalfName);
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith("-thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedThumbName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInputWithAllImages();

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
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(expectedThumbHalfName);
        output.ThumbFileUrl.Should().Be(expectedThumbName);
        output.BannerFileUrl.Should().Be(expectedBannerName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithMedia()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedMediaName = $"/storage/{_fixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedMediaName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInput(
            media: _fixture.GetValidMediaFileInput());

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
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.VideoFileUrl.Should().Be(expectedMediaName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithTrailer()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageServiceMock = new Mock<IStorageService>();
        var expectedTrailerName = $"/storage/{_fixture.GetValidMediaPath()}";
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(expectedTrailerName);
        var useCase = new UseCase.CreateVideo(
            repositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInput(
            trailer: _fixture.GetValidMediaFileInput());

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
        storageServiceMock.VerifyAll();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.TrailerFileUrl.Should().Be(expectedTrailerName);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionInUploadErrorCases()
    {
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()))
        .ThrowsAsync(new Exception("Something went wrong in upload"));
        var useCase = new UseCase.CreateVideo(
            Mock.Of<IVideoRepository>(),
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IUnitOfWork>(),
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInputWithAllImages();

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in upload");
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackUploadInImageUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionAndRollbackUploadInImageUploadErrorCases()
    {
        var storageServiceMock = new Mock<IStorageService>();
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync("123-banner.jpg");
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith("-thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync("123-thumb.jpg");
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith("-thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception("Something went wrong in upload"));
        var useCase = new UseCase.CreateVideo(
            Mock.Of<IVideoRepository>(),
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            Mock.Of<IUnitOfWork>(),
            storageServiceMock.Object
        );
        var input = _fixture.CreateValidInputWithAllImages();

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wrong in upload");
        storageServiceMock.Verify(
            x => x.Delete(
                It.Is<string>(x => (x == "123-banner.jpg") || (x == "123-thumb.jpg")),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackMediaUploadInCommitErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionAndRollbackMediaUploadInCommitErrorCases()
    {
        var input = _fixture.CreateValidInputWithAllMedias();
        var storageServiceMock = new Mock<IStorageService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var storageMediaPath = _fixture.GetValidMediaPath();
        var storageTrailerPath = _fixture.GetValidMediaPath();
        var storagePathList = new List<string>() { storageMediaPath, storageTrailerPath };
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith($"media.{input.Media!.Extension}")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(storageMediaPath);
        storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(x => x.EndsWith($"trailer.{input.Trailer!.Extension}")), It.IsAny<Stream>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(storageTrailerPath);
        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wront with the commit"));
        var useCase = new UseCase.CreateVideo(
            Mock.Of<IVideoRepository>(),
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            storageServiceMock.Object
        );

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Something went wront with the commit");
        storageServiceMock.Verify(
            x => x.Delete(
                It.Is<string>(path => storagePathList.Contains(path)),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
        storageServiceMock.Verify(
            x => x.Delete(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCategoriesIds()
    {
        var examplecategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(examplecategoriesIds);
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput(examplecategoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        List<Guid> outputItemCategoryIds = output.Categories
            .Select(categoryDto => categoryDto.Id).ToList();
        outputItemCategoryIds.Should().BeEquivalentTo(examplecategoriesIds);
        videoRepositoryMock.Verify(x => x.Insert(
        It.Is<DomainEntities.Video>(
            video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Categories.All(categoryId => examplecategoriesIds.Contains(categoryId))
            ),
            It.IsAny<CancellationToken>())
        );
        categoryRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenCategoryIdInvalid))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenCategoryIdInvalid()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var examplecategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedcategoryId = examplecategoriesIds[2];
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(examplecategoriesIds.FindAll(x => x!= removedcategoryId).ToList().AsReadOnly());
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput(examplecategoriesIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {removedcategoryId}.");
        categoryRepositoryMock.VerifyAll();
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
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
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

    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithGenresIds()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds);
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput(genresIds: exampleIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        var outputItemGenresIds = output.Genres
            .Select(dto => dto.Id).ToList();
        outputItemGenresIds.Should().BeEquivalentTo(exampleIds);
        videoRepositoryMock.Verify(x => x.Insert(
        It.Is<DomainEntities.Video>(
            video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.Genres.All(id => exampleIds.Contains(id))
            ),
            It.IsAny<CancellationToken>())
        );
        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidGenreId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenInvalidGenreId()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleIds[2];
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        genreRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds.FindAll(id => id != removedId));
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            categoryRepositoryMock.Object,
            genreRepositoryMock.Object,
            Mock.Of<ICastMemberRepository>(),
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput(genresIds: exampleIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {removedId}.");
        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCastMembersIds()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds);
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput(castMembersIds: exampleIds);

        var output = await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        var outputItemCastMembersIds = output.CastMembers
            .Select(dto => dto.Id).ToList();
        outputItemCastMembersIds.Should().BeEquivalentTo(exampleIds);
        videoRepositoryMock.Verify(x => x.Insert(
        It.Is<DomainEntities.Video>(
            video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened &&
                video.CastMembers.All(id => exampleIds.Contains(id))
            ),
            It.IsAny<CancellationToken>())
        );
        castMemberRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidCastMemberId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenInvalidCastMemberId()
    {
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var removedId = exampleIds[2];
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(exampleIds.FindAll(x => x != removedId));
        var useCase = new UseCase.CreateVideo(
            videoRepositoryMock.Object,
            Mock.Of<ICategoryRepository>(),
            Mock.Of<IGenreRepository>(),
            castMemberRepositoryMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<IStorageService>()
        );
        var input = _fixture.CreateValidInput(castMembersIds: exampleIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member id (or ids) not found: {removedId}.");
        castMemberRepositoryMock.VerifyAll();
    }
}
