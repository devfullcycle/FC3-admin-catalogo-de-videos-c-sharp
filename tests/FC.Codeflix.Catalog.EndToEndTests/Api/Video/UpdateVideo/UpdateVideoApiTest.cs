using FC.Codeflix.Catalog.Api.ApiModels.Video;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.UpdateVideo;

[Collection(nameof(VideoBaseFixture))]
public class UpdateVideoApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public UpdateVideoApiTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateVideo))]
    [Trait("EndToEnd/Api", "Video/UpdateVideo - Endpoints")]
    public async Task UpdateVideo()
    {
        var exampleVideos = _fixture.GetVideoCollection(10);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var targetVideoId = exampleVideos.ElementAt(5).Id;

        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating =_fixture.GetRandomRating().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched()
        };

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<VideoModelOutput>>(
                $"/videos/{targetVideoId}",
                input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Id.Should().Be(targetVideoId);
        output.Data.Title.Should().Be(input!.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        var videoFromDb = await _fixture.VideoPersistence.GetById(targetVideoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(targetVideoId);
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!.ToRating());
    }

    [Fact(DisplayName = nameof(UpdateVideoWithRelationships))]
    [Trait("EndToEnd/Api", "Video/UpdateVideo - Endpoints")]
    public async Task UpdateVideoWithRelationships()
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

        var targetVideoId = exampleVideos.ElementAt(5).Id;
        var targetCategories = new[]
        {
            exampleCategories.ElementAt(1)
        };
        var targetGenres = new[]
        {
            exampleGenres.ElementAt(0),
            exampleGenres.ElementAt(2)
        };
        var targetCastMembers = new[]
        {
            exampleCastMembers.ElementAt(1),
            exampleCastMembers.ElementAt(2),
            exampleCastMembers.ElementAt(3)
        };

        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRating().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched(),
            CategoriesIds = targetCategories.Select(x => x.Id).ToList(),
            GenresIds = targetGenres.Select(x => x.Id).ToList(),
            CastMembersIds = targetCastMembers.Select(x => x.Id).ToList()
        };

        var (response, output) = await _fixture.ApiClient
            .Put<TestApiResponse<VideoModelOutput>>(
                $"/videos/{targetVideoId}",
                input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data!.Id.Should().Be(targetVideoId);
        output.Data.Title.Should().Be(input!.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        var expectedCategories = targetCategories
            .Select(category => new VideoModelOutputRelatedAggregate(category.Id));
        output.Data.Categories.Should().BeEquivalentTo(exampleCategories);
        var expectedGenres = targetGenres
            .Select(genre => new VideoModelOutputRelatedAggregate(genre.Id));
        output.Data.Genres.Should().BeEquivalentTo(expectedGenres);
        var expectedCastMembers = targetCastMembers
            .Select(castMember => new VideoModelOutputRelatedAggregate(castMember.Id));
        output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
        var videoFromDb = await _fixture.VideoPersistence.GetById(targetVideoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().Be(targetVideoId);
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!.ToRating());
        var categoriesFromDb = await _fixture.VideoPersistence
            .GetVideosCategories(targetVideoId);
        var categoriesIdsFromDb = categoriesFromDb.Select(x => x.CategoryId);
        input.CategoriesIds.Should().BeEquivalentTo(categoriesIdsFromDb);
        var genresFromDb = await _fixture.VideoPersistence
            .GetVideosGenres(targetVideoId);
        var genresIdsFromDb = genresFromDb.Select(x => x.GenreId);
        input.GenresIds.Should().BeEquivalentTo(genresIdsFromDb);
        var castMembersFromDb = await _fixture.VideoPersistence
            .GetVideosCastMembers(targetVideoId);
        var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId);
        input.CastMembersIds.Should().BeEquivalentTo(castMembersIdsFromDb);
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/Api", "Video/UpdateVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoCollection(10);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = Guid.NewGuid();
        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRating().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched()
        };

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{videoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");
    }

    [Fact(DisplayName = nameof(Error422WhenCategoryIdNotFound))]
    [Trait("EndToEnd/Api", "Video/UpdateVideo - Endpoints")]
    public async Task Error422WhenCategoryIdNotFound()
    {
        var exampleVideos = _fixture.GetVideoCollection(10);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos.ElementAt(4).Id;
        var categoryId = Guid.NewGuid();
        var input = new UpdateVideoApiInput
        {
            Title = _fixture.GetValidTitle(),
            Description = _fixture.GetValidDescription(),
            Duration = _fixture.GetValidDuration(),
            Opened = _fixture.GetRandomBoolean(),
            Published = _fixture.GetRandomBoolean(),
            Rating = _fixture.GetRandomRating().ToStringSignal(),
            YearLaunched = _fixture.GetValidYearLaunched(),
            CategoriesIds = new List<Guid> { categoryId }
        };

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/videos/{videoId}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {categoryId}.");
    }

    public void Dispose() => _fixture.CleanPersistence();
}
