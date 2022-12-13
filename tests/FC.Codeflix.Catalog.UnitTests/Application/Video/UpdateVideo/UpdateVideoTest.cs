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
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.Domain.Exceptions;
using System.Linq;
using System.Collections.Generic;
using FC.Codeflix.Catalog.Domain.Entity;
using System.IO;
using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepository;
    private readonly Mock<IGenreRepository> _genreRepository;
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<ICastMemberRepository> _castMemberRepository;
    private readonly Mock<IUnitOfWork> _unitofWork;
    private readonly Mock<IStorageService> _storageService;
    private readonly UseCase.UpdateVideo _useCase;

    public UpdateVideoTest(UpdateVideoTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepository = new();
        _genreRepository = new();
        _categoryRepository = new();
        _castMemberRepository = new();
        _unitofWork = new();
        _storageService = new();
        _useCase = new(_videoRepository.Object,
            _genreRepository.Object,
            _categoryRepository.Object,
            _castMemberRepository.Object,
            _unitofWork.Object,
            _storageService.Object);
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
        output.Should().NotBeNull();
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

    [Fact(DisplayName = nameof(UpdateVideosWithGenreIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithGenreIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, examplesGenreIds);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesGenreIds.Count &&
                    idsList.All(id => examplesGenreIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _genreRepository.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                ((video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                video.Genres.All(genreId => examplesGenreIds.Contains(genreId) &&
                (video.Genres.Count == examplesGenreIds.Count))))
            , It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(examplesGenreIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithoutRelationsWithRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithoutRelationsWithRelations()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCastMembersIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, 
            genreIds: examplesGenreIds,
            categoryIds: examplesCategoriesIds,
            castMemberIds: examplesCastMembersIds);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesGenreIds.Count &&
                    idsList.All(id => examplesGenreIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);
        _castMemberRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesCastMembersIds.Count &&
                    idsList.All(id => examplesCastMembersIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCastMembersIds);
        _categoryRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesCategoriesIds.Count &&
                    idsList.All(id => examplesCategoriesIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCategoriesIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _genreRepository.VerifyAll();
        _categoryRepository.VerifyAll();
        _castMemberRepository.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                video.Genres.All(genreId => examplesGenreIds.Contains(genreId)) &&
                (video.Genres.Count == examplesGenreIds.Count) &&
                video.Categories.All(id => examplesCategoriesIds.Contains(id)) &&
                (video.Categories.Count == examplesCategoriesIds.Count) &&
                video.CastMembers.All(id => examplesCastMembersIds.Contains(id)) &&
                (video.CastMembers.Count == examplesCastMembersIds.Count)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(examplesGenreIds);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(examplesCategoriesIds);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(examplesCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsToOtherRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithRelationsToOtherRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCastMembersIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var examplesCategoriesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: examplesGenreIds,
            categoryIds: examplesCategoriesIds,
            castMemberIds: examplesCastMembersIds);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesGenreIds.Count &&
                    idsList.All(id => examplesGenreIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);
        _castMemberRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesCastMembersIds.Count &&
                    idsList.All(id => examplesCastMembersIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCastMembersIds);
        _categoryRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == examplesCategoriesIds.Count &&
                    idsList.All(id => examplesCategoriesIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesCategoriesIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _genreRepository.VerifyAll();
        _categoryRepository.VerifyAll();
        _castMemberRepository.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                video.Genres.All(genreId => examplesGenreIds.Contains(genreId)) &&
                (video.Genres.Count == examplesGenreIds.Count) &&
                video.Categories.All(id => examplesCategoriesIds.Contains(id)) &&
                (video.Categories.Count == examplesCategoriesIds.Count) &&
                video.CastMembers.All(id => examplesCastMembersIds.Contains(id)) &&
                (video.CastMembers.Count == examplesCastMembersIds.Count)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(examplesGenreIds);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(examplesCategoriesIds);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(examplesCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsRemovingRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithRelationsRemovingRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: new(),
            categoryIds: new(),
            castMemberIds: new());
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Should().BeEmpty();
        output.Categories.Should().BeEmpty();
        output.CastMembers.Should().BeEmpty();
        _videoRepository.VerifyAll();
        _genreRepository.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _categoryRepository.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _castMemberRepository.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never); ;
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.Genres.Count == 0) &&
                (video.Categories.Count == 0) &&
                (video.CastMembers.Count == 0)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsKeepRelationWhenReceiveNullInRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithRelationsKeepRelationWhenReceiveNullInRelations()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            genreIds: null,
            categoryIds: null,
            castMemberIds: null);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(genre => genre.Id).ToList()
            .Should().BeEquivalentTo(exampleVideo.Genres);
        output.Categories.Select(category => category.Id).ToList()
            .Should().BeEquivalentTo(exampleVideo.Categories);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(exampleVideo.CastMembers);
        _videoRepository.VerifyAll();
        _genreRepository.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _categoryRepository.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _castMemberRepository.Verify(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        ), Times.Never); ;
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.Genres.Count == exampleVideo.Genres.Count) &&
                (video.Genres.All(id => exampleVideo.Genres.Contains(id))) &&
                (video.Categories.Count == exampleVideo.Categories.Count) &&
                (video.Categories.All(id => exampleVideo.Categories.Contains(id))) &&
                (video.CastMembers.Count == exampleVideo.CastMembers.Count) &&
                (video.CastMembers.All(id => exampleVideo.CastMembers.Contains(id)))
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithCategoryIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithCategoryIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, categoryIds: exampleIds);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleIds.Count &&
                    idsList.All(id => exampleIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _genreRepository.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                ((video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                video.Categories.All(genreId => exampleIds.Contains(genreId) &&
                (video.Categories.Count == exampleIds.Count))))
            , It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Select(categrory => categrory.Id).ToList()
            .Should().BeEquivalentTo(exampleIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithCastMemberIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithCastMemberIds()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, 
            castMemberIds: exampleIds);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepository.Setup(x =>
            x.GetIdsListByIds(
                It.Is<List<Guid>>(idsList =>
                    idsList.Count == exampleIds.Count &&
                    idsList.All(id => exampleIds.Contains(id))),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var output = await _useCase.Handle(input, CancellationToken.None);

        _videoRepository.VerifyAll();
        _genreRepository.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                ((video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                video.CastMembers.All(genreId => exampleIds.Contains(genreId) &&
                (video.CastMembers.Count == exampleIds.Count))))
            , It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CastMembers.Select(castMember => castMember.Id).ToList()
            .Should().BeEquivalentTo(exampleIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidCastMemberId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenInvalidGenreId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidId = Guid.NewGuid();
        var inputInvalidIdsList = examplesIds
            .Concat(new List<Guid>() { invalidId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, 
            castMemberIds: inputInvalidIdsList);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _castMemberRepository.Setup(x =>
            x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related cast member(s) id (or ids) not found: {invalidId}.");
        _videoRepository.VerifyAll();
        _castMemberRepository.VerifyAll();
        _unitofWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidCategoryId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenInvalidCategoryId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidId = Guid.NewGuid();
        var inputInvalidIdsList = exampleIds
            .Concat(new List<Guid>() { invalidId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, 
            categoryIds: inputInvalidIdsList);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _categoryRepository.Setup(x =>
            x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {invalidId}.");
        _videoRepository.VerifyAll();
        _categoryRepository.VerifyAll();
        _unitofWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidGenreId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenInvalidCastMemberId()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var examplesGenreIds = Enumerable.Range(1, 5)
            .Select(_ => Guid.NewGuid()).ToList();
        var invalidGenreId = Guid.NewGuid();
        var inputInvalidIdsList = examplesGenreIds
            .Concat(new List<Guid>() { invalidGenreId }).ToList();
        var input = _fixture.CreateValidInput(exampleVideo.Id, inputInvalidIdsList);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _genreRepository.Setup(x =>
            x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(examplesGenreIds);

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related genre id (or ids) not found: {invalidGenreId}.");
        _videoRepository.VerifyAll();
        _genreRepository.VerifyAll();
        _unitofWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }


    [Theory(DisplayName = nameof(UpdateVideosThrowsWhenReceiveInvalidInput))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    [ClassData(typeof(UpdateVideoTestDataGenerator))]
    public async Task UpdateVideosThrowsWhenReceiveInvalidInput(
        UpdateVideoInput invalidinput,
        string expectedExceptionMessage)
    {
        var exampleVideo = _fixture.GetValidVideo();
        _videoRepository.Setup(repository => repository
            .Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var action = () => _useCase.Handle(invalidinput, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should()
            .Be(expectedExceptionMessage);
        _videoRepository.VerifyAll();
        _unitofWork.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenVideoNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenVideoNotFound()
    {
        var input = _fixture.CreateValidInput(Guid.NewGuid());
        _videoRepository.Setup(repository =>
            repository.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new NotFoundException("Video not found"));

        var action = () => _useCase.Handle(input, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Video not found");

        _videoRepository.Verify(repository => repository.Update(
            It.IsAny<DomainEntities.Video>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithBannerWhenVideoHasNoBanner))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithBannerWhenVideoHasNoBanner()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            banner: _fixture.GetValidImageFileInput());
        var bannerPath = $"storage/banner.{input.Banner!.Extension}";
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => (name == StorageFileName.Create(
                exampleVideo.Id,
                nameof(exampleVideo.Banner),
                input.Banner!.Extension))
            ),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(bannerPath);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(bannerPath);
        _videoRepository.VerifyAll();
        _storageService.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.Banner!.Path == bannerPath)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepBannerWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepBannerWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            banner: null);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner!.Path);
        _videoRepository.VerifyAll();
        _storageService.Verify(x => x.Upload(
            It.IsAny<string>(), 
            It.IsAny<Stream>(), 
            It.IsAny<CancellationToken>())
        , Times.Never);
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.Banner!.Path == exampleVideo.Banner!.Path)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithThumbWhenVideoHasNoThumb))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithThumbWhenVideoHasNoThumb()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumb: _fixture.GetValidImageFileInput());
        var path = $"storage/thumb.{input.Thumb!.Extension}";
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => (name == StorageFileName.Create(
                exampleVideo.Id,
                nameof(exampleVideo.Thumb),
                input.Thumb!.Extension))
            ),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(path);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(path);
        _videoRepository.VerifyAll();
        _storageService.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.Thumb!.Path == path)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepThumbWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepThumbWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumb: null);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb!.Path);
        _videoRepository.VerifyAll();
        _storageService.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>())
        , Times.Never);
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.Thumb!.Path == exampleVideo.Thumb!.Path)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithThumbHalfWhenVideoHasNoThumbHalf))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithThumbHalfWhenVideoHasNoThumbHalf()
    {
        var exampleVideo = _fixture.GetValidVideo();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumbHalf: _fixture.GetValidImageFileInput());
        var path = $"storage/thumb-half.{input.ThumbHalf!.Extension}";
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        _storageService.Setup(x => x.Upload(
            It.Is<string>(name => (name == StorageFileName.Create(
                exampleVideo.Id,
                nameof(exampleVideo.ThumbHalf),
                input.ThumbHalf!.Extension))
            ),
            It.IsAny<MemoryStream>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(path);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(path);
        _videoRepository.VerifyAll();
        _storageService.VerifyAll();
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.ThumbHalf!.Path == path)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepThumbHalfWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepThumbHalfWhenReceiveNull()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.CreateValidInput(exampleVideo.Id,
            thumbHalf: null);
        _videoRepository.Setup(repository =>
            repository.Get(
                It.Is<Guid>(id => id == exampleVideo.Id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalfFileUrl.Should().Be(exampleVideo.ThumbHalf!.Path);
        _videoRepository.VerifyAll();
        _storageService.Verify(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>())
        , Times.Never);
        _videoRepository.Verify(repository => repository.Update(
            It.Is<DomainEntities.Video>(video =>
                (video.Id == exampleVideo.Id) &&
                (video.Title == input.Title) &&
                (video.Description == input.Description) &&
                (video.Rating == input.Rating) &&
                (video.YearLaunched == input.YearLaunched) &&
                (video.Opened == input.Opened) &&
                (video.Published == input.Published) &&
                (video.Duration == input.Duration) &&
                (video.ThumbHalf!.Path == exampleVideo.ThumbHalf!.Path)
            ), It.IsAny<CancellationToken>())
        , Times.Once);
        _unitofWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }
}