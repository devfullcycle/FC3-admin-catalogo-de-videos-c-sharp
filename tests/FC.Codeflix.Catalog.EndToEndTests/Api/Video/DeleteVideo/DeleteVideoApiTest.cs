using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FluentAssertions;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.DeleteVideo;

[Collection(nameof(VideoBaseFixture))]
public class DeleteVideoApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public DeleteVideoApiTest(VideoBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("EndToEnd/Api", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideo()
    {
        var examples = _fixture.GetVideoCollection(10);
        await _fixture.VideoPersistence.InsertList(examples);
        var mediaCount = await _fixture.VideoPersistence.GetMediaCount();
        var expectedMediaCount = mediaCount - 2;
        var video = examples[7];
        var videoId = video.Id;
        var allMedias = new[]
        {
            video.Trailer!.FilePath,
            video.Media!.FilePath,
            video.Banner!.Path,
            video.Thumb!.Path,
            video.ThumbHalf!.Path
        };

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDB = await _fixture.VideoPersistence
            .GetById(videoId);
        videoFromDB.Should().BeNull();
        var actualMediaCount = await _fixture.VideoPersistence
            .GetMediaCount();
        actualMediaCount.Should().Be(expectedMediaCount);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.DeleteObjectAsync(
                It.IsAny<string>(),
                It.Is<string>(fileName => allMedias.Contains(fileName)),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.DeleteObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithRelationships))]
    [Trait("EndToEnd/Api", "Video/DeleteVideo - Endpoints")]
    public async Task DeleteVideoWithRelationships()
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
        var mediaCount = await _fixture.VideoPersistence.GetMediaCount();
        var expectedMediaCount = mediaCount - 2;

        var video = exampleVideos[7];
        var videoId = video.Id;
        var allMedias = new[]
        {
            video.Trailer!.FilePath,
            video.Media!.FilePath,
            video.Banner!.Path,
            video.Thumb!.Path,
            video.ThumbHalf!.Path
        };

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDB = await _fixture.VideoPersistence
            .GetById(videoId);
        videoFromDB.Should().BeNull();
        var categoriesFromDB = await _fixture.VideoPersistence
            .GetVideosCategories(videoId);
        categoriesFromDB.Should().BeEmpty();
        var genresFromDB = await _fixture.VideoPersistence
            .GetVideosGenres(videoId);
        genresFromDB.Should().BeEmpty();
        var castMembersFromDB = await _fixture.VideoPersistence
            .GetVideosCastMembers(videoId);
        castMembersFromDB.Should().BeEmpty();
        var actualMediaCount = await _fixture.VideoPersistence
            .GetMediaCount();
        actualMediaCount.Should().Be(expectedMediaCount);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.DeleteObjectAsync(
                It.IsAny<string>(),
                It.Is<string>(fileName => allMedias.Contains(fileName)),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.DeleteObjectAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DeleteObjectOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(5));
    }

    [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
    [Trait("EndToEnd/Api", "Video/DeleteVideo - Endpoints")]
    public async Task Error404WhenVideoIdNotFound()
    {
        var examples = _fixture.GetVideoCollection(2);
        await _fixture.VideoPersistence.InsertList(examples);

        var videoId = Guid.NewGuid();

        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/videos/{videoId}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Video '{videoId}' not found.");
    }

    public void Dispose() => _fixture.CleanPersistence();
}
