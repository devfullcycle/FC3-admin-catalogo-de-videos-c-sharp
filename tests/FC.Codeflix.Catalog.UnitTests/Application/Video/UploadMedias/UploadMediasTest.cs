using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using Xunit;
using System;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[Collection(nameof(UploadMediasTestFixture))]
public class UploadMediasTest
{
    private readonly UploadMediasTestFixture _fixture;
    private readonly UseCase.UploadMedias _useCase;
    private readonly Mock<IVideoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;

    public UploadMediasTest(UploadMediasTestFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IVideoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageServiceMock = new Mock<IStorageService>();
        _useCase = new UseCase.UploadMedias(
            _repositoryMock.Object, 
            _storageServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact(DisplayName = nameof(UploadMedias))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task UploadMedias()
    {
        _repositoryMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(_fixture.GetValidVideo());
        _storageServiceMock
            .Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(_fixture.GetValidInput(), CancellationToken.None);

        _repositoryMock.VerifyAll();
        _storageServiceMock.Verify(x => 
            x.Upload(It.IsAny<string>(), It.IsAny<Stream>(),It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }
}
