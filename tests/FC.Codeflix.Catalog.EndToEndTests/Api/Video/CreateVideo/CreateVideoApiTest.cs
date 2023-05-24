using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Api.ApiModels.Video;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Extensions;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.CreateVideo;
[Collection(nameof(VideoBaseFixture))]
public class CreateVideoApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public CreateVideoApiTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateBasicVideo))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateBasicVideo()
    {
        CreateVideoApiInput input = _fixture.GetBasicCreateVideoInput();

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.Title.Should().Be(input.Title);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!.ToRating());
    }

    [Fact(DisplayName = nameof(CreateVideoWithRelationships))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithRelationships()
    {
        var categories = _fixture.GetExampleCategoriesList();
        await _fixture.CategoryPersistence.InsertList(categories);

        var genres = _fixture.GetExampleListGenres();
        await _fixture.GenrePersistence.InsertList(genres);

        var castMembers = _fixture.GetExampleCastMembersList();
        await _fixture.CastMemberPersistence.InsertList(castMembers);

        CreateVideoApiInput input = _fixture.GetBasicCreateVideoInput();
        input.CategoriesIds = categories.Select(c => c.Id).ToList();
        input.GenresIds = genres.Select(c => c.Id).ToList();
        input.CastMembersIds = castMembers.Select(c => c.Id).ToList();

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<VideoModelOutput>>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Description.Should().Be(input.Description);
        output.Data.Title.Should().Be(input.Title);
        output.Data.YearLaunched.Should().Be(input.YearLaunched);
        output.Data.Opened.Should().Be(input.Opened);
        output.Data.Published.Should().Be(input.Published);
        output.Data.Duration.Should().Be(input.Duration);
        output.Data.Rating.Should().Be(input.Rating);
        var outputCategoryIds = output.Data.Categories.Select(c => c.Id).ToList();
        outputCategoryIds.Should().NotBeEmpty();
        outputCategoryIds.Should().BeEquivalentTo(input.CategoriesIds);
        var outputGenreIds = output.Data.Genres.Select(c => c.Id).ToList();
        outputGenreIds.Should().NotBeEmpty();
        outputGenreIds.Should().BeEquivalentTo(input.GenresIds);
        var outputCastMemberIds = output.Data.CastMembers.Select(c => c.Id).ToList();
        outputCastMemberIds.Should().NotBeEmpty();
        outputCastMemberIds.Should().BeEquivalentTo(input.CastMembersIds);
        var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Id.Should().NotBeEmpty();
        videoFromDb.Title.Should().Be(input.Title);
        videoFromDb.Description.Should().Be(input.Description);
        videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
        videoFromDb.Opened.Should().Be(input.Opened);
        videoFromDb.Published.Should().Be(input.Published);
        videoFromDb.Duration.Should().Be(input.Duration);
        videoFromDb.Rating.Should().Be(input.Rating!.ToRating());
        var categoriesFromDb = await _fixture.VideoPersistence
            .GetVideosCategories(videoFromDb.Id);
        categoriesFromDb.Should().NotBeNull();
        var categoriesIdsFromDb = categoriesFromDb.Select(x => x.CategoryId);
        categoriesIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);
        var genresFromDb = await _fixture.VideoPersistence
            .GetVideosGenres(videoFromDb.Id);
        genresFromDb.Should().NotBeNull();
        var genresIdsFromDb = genresFromDb.Select(x => x.GenreId);
        genresIdsFromDb.Should().BeEquivalentTo(input.GenresIds);
        var castMembersFromDb = await _fixture.VideoPersistence
            .GetVideosCastMembers(videoFromDb.Id);
        castMembersFromDb.Should().NotBeNull();
        var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId);
        castMembersIdsFromDb.Should().BeEquivalentTo(input.CastMembersIds);
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidGenreId))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidGenreId()
    {
        var invalidGenreId = Guid.NewGuid();
        CreateVideoApiInput input = _fixture.GetBasicCreateVideoInput();
        input.GenresIds = new List<Guid> { invalidGenreId };

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related genre id (or ids) not found: {invalidGenreId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCategoryId))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCategoryId()
    {
        var invalidCategoryId = Guid.NewGuid();
        CreateVideoApiInput input = _fixture.GetBasicCreateVideoInput();
        input.CategoriesIds = new List<Guid> { invalidCategoryId };

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category id (or ids) not found: {invalidCategoryId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithInvalidCastMemberId))]
    [Trait("EndToEnd/Api", "Video/CreateVideo - Endpoints")]
    public async Task CreateVideoWithInvalidCastMemberId()
    {
        var invalidCastMemberId = Guid.NewGuid();
        CreateVideoApiInput input = _fixture.GetBasicCreateVideoInput();
        input.CastMembersIds = new List<Guid> { invalidCastMemberId };

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/videos", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related cast member id (or ids) not found: {invalidCastMemberId}.");
    }

    public void Dispose() => _fixture.CleanPersistence();
}
