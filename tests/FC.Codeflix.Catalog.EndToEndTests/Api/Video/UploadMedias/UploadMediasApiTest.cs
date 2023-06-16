using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.Stream;
using FluentAssertions;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.UploadMedias;

[Collection(nameof(VideoBaseFixture))]
public class UploadMediasApiTest : IDisposable
{
    private readonly VideoBaseFixture _fixture;

    public UploadMediasApiTest(VideoBaseFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UploadBanner))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadBanner()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "banner";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(DomainEntities.Video.Banner), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Banner!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(
                It.IsAny<string>(), expectedFileName,
                file.ContentType,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumb))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadThumb()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(DomainEntities.Video.Thumb), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.Thumb!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(
                It.IsAny<string>(), expectedFileName,
                file.ContentType,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()),
            Times.Once);
    }

    [Fact(DisplayName = nameof(UploadThumbHalf))]
    [Trait("EndToEnd/Api", "Video/UploadMedias - Endpoints")]
    public async Task UploadThumbHalf()
    {
        var exampleVideos = _fixture.GetVideoCollection(5);
        await _fixture.VideoPersistence.InsertList(exampleVideos);

        var videoId = exampleVideos[2].Id;
        var mediaType = "thumbnail_half";
        var file = _fixture.GetValidImageFileInput();
        var expectedFileName = StorageFileName.Create(videoId,
            nameof(DomainEntities.Video.ThumbHalf), file.Extension);
        var expectedContent = file.FileStream.ToContentString();

        var (response, output) = await _fixture.ApiClient
            .PostFormData<object>(
                $"/videos/{videoId}/medias/{mediaType}",
                file);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var videoFromDb = await _fixture.VideoPersistence.GetById(videoId);
        videoFromDb.Should().NotBeNull();
        videoFromDb!.ThumbHalf!.Path.Should().Be(expectedFileName);
        _fixture.WebAppFactory.StorageClient!.Verify(
            x => x.UploadObjectAsync(
                It.IsAny<string>(), expectedFileName,
                file.ContentType,
                It.Is<Stream>(stream => stream.ToContentString() == expectedContent),
                It.IsAny<UploadObjectOptions>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<IProgress<IUploadProgress>>()),
            Times.Once);
    }

    public void Dispose() => _fixture.CleanPersistence();
}
