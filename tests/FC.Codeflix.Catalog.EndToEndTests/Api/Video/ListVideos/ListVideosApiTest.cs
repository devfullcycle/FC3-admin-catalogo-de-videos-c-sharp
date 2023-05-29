using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.ListVideos;

[Collection(nameof(VideoBaseFixture))]
public class ListVideosApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public ListVideosApiTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListVideos))]
    [Trait("EndToEnd/Api", "Video/ListVideos - Endpoints")]
    public async Task ListVideos()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(3);
        var exampleGenres = _fixture.GetExampleListGenres(4);
        var exampleCastMembers = _fixture.GetExampleCastMembersList(5);
        var exampleVideos = _fixture.GetVideoCollection(10);

        exampleVideos.ForEach(video =>
        {
            exampleCategories.ForEach(category
                => video.AddCategory(category.Id));
            exampleGenres.ForEach(genre => video.AddGenre(genre.Id));
            exampleCastMembers.ForEach(castMember
                => video.AddCastMember(castMember.Id));
        });

        await _fixture.CategoryPersistence.InsertList(exampleCategories);
        await _fixture.GenrePersistence.InsertList(exampleGenres);
        await _fixture.CastMemberPersistence.InsertList(exampleCastMembers);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var input = new ListVideosInput
        {
            Page = 1,
            PerPage = exampleVideos.Count
        };
        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output.Data.Should().NotBeNull();
        output.Data!.Count.Should().Be(exampleVideos.Count);
        output.Data.ForEach(outputItem =>
        {
            var exampleItem = exampleVideos
                .Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem!.Title);
            outputItem.Description.Should().Be(exampleItem!.Description);
            outputItem.YearLaunched.Should().Be(exampleItem!.YearLaunched);
            outputItem.Opened.Should().Be(exampleItem!.Opened);
            outputItem.Published.Should().Be(exampleItem!.Published);
            outputItem.Duration.Should().Be(exampleItem!.Duration);
            outputItem.Rating.Should().Be(exampleItem!.Rating.ToStringSignal());
            var expectedCategories = exampleCategories
                .Select(category => new VideoModelOutputRelatedAggregate(
                    category.Id, category.Name));
            outputItem.Categories.Should().BeEquivalentTo(expectedCategories);
            var expectedGenres = exampleGenres
                .Select(genre => new VideoModelOutputRelatedAggregate(
                    genre.Id, genre.Name));
            outputItem.Genres.Should().BeEquivalentTo(expectedGenres);
            var expectedCastMembers = exampleCastMembers
                .Select(castMember => new VideoModelOutputRelatedAggregate(
                    castMember.Id));
            outputItem.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
        });
    }

    [Fact(DisplayName = nameof(ReturnsEmptyWhenThereIsNoVideo))]
    [Trait("EndToEnd/Api", "Video/ListVideos - Endpoints")]
    public async Task ReturnsEmptyWhenThereIsNoVideo()
    {
        var input = new ListVideosInput
        {
            Page = 1,
            PerPage = 10
        };
        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(0);
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta!.PerPage.Should().Be(input.PerPage);
        output!.Data.Should().NotBeNull();
        output!.Data.Should().BeEmpty();
    }

    public void Dispose() => _fixture.CleanPersistence();
}
