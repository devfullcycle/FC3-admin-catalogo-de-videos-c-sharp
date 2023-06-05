using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.GetVideo;

[Collection(nameof(VideoBaseFixture))]
public class GetVideoApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public GetVideoApiTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetVideo))]
    [Trait("EndToEnd/Api", "Video/GetVideo - Endpoints")]
    public async Task GetVideo()
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

        var exampleItem = exampleVideos.ElementAt(5);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponse<VideoModelOutput>>($"/videos/{exampleItem.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Id.Should().Be(exampleItem!.Id);
        output.Data.Title.Should().Be(exampleItem!.Title);
        output.Data.Description.Should().Be(exampleItem!.Description);
        output.Data.YearLaunched.Should().Be(exampleItem!.YearLaunched);
        output.Data.Opened.Should().Be(exampleItem!.Opened);
        output.Data.Published.Should().Be(exampleItem!.Published);
        output.Data.Duration.Should().Be(exampleItem!.Duration);
        output.Data.Rating.Should().Be(exampleItem!.Rating.ToStringSignal());
        var expectedCategories = exampleCategories
            .Select(category => new VideoModelOutputRelatedAggregate(
                category.Id, null));
        output.Data.Categories.Should().BeEquivalentTo(expectedCategories);
        var expectedGenres = exampleGenres
            .Select(genre => new VideoModelOutputRelatedAggregate(
                genre.Id, null));
        output.Data.Genres.Should().BeEquivalentTo(expectedGenres);
        var expectedCastMembers = exampleCastMembers
            .Select(castMember => new VideoModelOutputRelatedAggregate(
                castMember.Id, null));
        output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
    }

    public void Dispose() => _fixture.CleanPersistence();
}
