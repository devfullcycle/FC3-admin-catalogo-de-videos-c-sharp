using FC.Codeflix.Catalog.Domain.Repository;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using Xunit;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.Common;
using FluentAssertions;
using System.Linq;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

[Collection(nameof(ListVideosTestFixture))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    private readonly UseCase.ListVideos _useCase;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        _fixture = fixture;
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _useCase = new UseCase.ListVideos(_videoRepositoryMock.Object);
    }

    [Fact(DisplayName = nameof(ListVideos))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListVideos()
    {
        var exampleVideosList = _fixture.CreateExampleVideosList();
        var input = new UseCase.ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        _videoRepositoryMock.Setup(x =>
            x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            new SearchOutput<DomainEntities.Video>(
                input.Page, 
                input.PerPage, 
                exampleVideosList.Count, 
                exampleVideosList));

        PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(outputItem => {
            var exampleVideo = exampleVideosList.Find(x => x.Id == outputItem.Id);
            exampleVideo.Should().NotBeNull();
            output.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleVideo!.Id);
            outputItem.CreatedAt.Should().Be(exampleVideo.CreatedAt);
            outputItem.Title.Should().Be(exampleVideo.Title);
            outputItem.Published.Should().Be(exampleVideo.Published);
            outputItem.Description.Should().Be(exampleVideo.Description);
            outputItem.Duration.Should().Be(exampleVideo.Duration);
            outputItem.Rating.Should().Be(exampleVideo.Rating);
            outputItem.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            outputItem.Opened.Should().Be(exampleVideo.Opened);
            outputItem.Thumb.Should().Be(exampleVideo.Thumb!.Path);
            outputItem.ThumbHalf.Should().Be(exampleVideo.ThumbHalf!.Path);
            outputItem.Banner.Should().Be(exampleVideo.Banner!.Path);
            outputItem.Media.Should().Be(exampleVideo.Media!.FilePath);
            outputItem.Trailer.Should().Be(exampleVideo.Trailer!.FilePath);
            outputItem.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
            outputItem.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
            outputItem.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
        });
        _videoRepositoryMock.VerifyAll();
    }


    [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereIsNoVideo))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListReturnsEmptyWhenThereIsNoVideo()
    {
        var input = new UseCase.ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        _videoRepositoryMock.Setup(x =>
            x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            new SearchOutput<DomainEntities.Video>(
                input.Page,
                input.PerPage,
                0,
                new List<DomainEntities.Video>()));

        PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        _videoRepositoryMock.VerifyAll();
    }
}

