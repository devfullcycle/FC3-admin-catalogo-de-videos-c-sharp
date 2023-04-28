using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using FC.Codeflix.Catalog.Infra.Storage.Services;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using GcpData = Google.Apis.Storage.v1.Data;

namespace FC.Codeflix.Catalog.UnitTests.Infra.Storage;

[Collection(nameof(StorageServiceTestFixture))]
public class StorageServiceTest
{
    private readonly StorageServiceTestFixture _fixture;

    public StorageServiceTest(StorageServiceTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Upload))]
    [Trait("Infra.Storage", "StorageService")]
    public async Task Upload()
    {
        var storageClientMock = new Mock<StorageClient>();
        var objectMock = new Mock<GcpData.Object>();
        storageClientMock.Setup(x => x.UploadObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Stream>(), It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(), It.IsAny<IProgress<IUploadProgress>>()))
        .ReturnsAsync(objectMock.Object);
        var storageOptions = new StorageServiceOptions
        {
            BucketName = _fixture.GetBucketName()
        };
        var options = Options.Create(storageOptions);
        var service = new StorageService(
            storageClientMock.Object, options);
        var fileName = _fixture.GetFileName();
        var contentStream = Encoding.UTF8.GetBytes(_fixture.GetContentFile());
        var stream = new MemoryStream(contentStream);
        var contentType = _fixture.GetContentType();

        var filePath = await service.Upload(
            fileName, stream, contentType, CancellationToken.None);

        Assert.Equal(fileName, filePath);
        storageClientMock.Verify(x => x.UploadObjectAsync(
            storageOptions.BucketName, fileName, contentType,
            stream, It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(), It.IsAny<IProgress<IUploadProgress>>()),
        Times.Once());
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Infra.Storage", "StorageService")]
    public async Task Delete()
    {
        var storageClientMock = new Mock<StorageClient>();
        storageClientMock.Setup(x => x.DeleteObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<DeleteObjectOptions>(), It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);
        var storageOptions = new StorageServiceOptions
        {
            BucketName = _fixture.GetBucketName()
        };
        var options = Options.Create(storageOptions);
        var service = new StorageService(
            storageClientMock.Object, options);
        var fileName = _fixture.GetFileName();

        await service.Delete(fileName, CancellationToken.None);
        storageClientMock.Verify(x => x.DeleteObjectAsync(
            storageOptions.BucketName, fileName,
            It.IsAny<DeleteObjectOptions>(), It.IsAny<CancellationToken>()),
        Times.Once());
    }
}
