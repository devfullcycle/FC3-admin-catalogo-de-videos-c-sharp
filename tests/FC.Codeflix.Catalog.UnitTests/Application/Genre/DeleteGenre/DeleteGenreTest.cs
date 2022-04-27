using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.DeleteGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCase.DeleteGenreInput(
            exampleGenre.Id
        );

        await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(
            x => x.Get(
                It.Is<Guid>(x => x == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        genreRepositoryMock.Verify(
            x => x.Delete(
                It.Is<DomainEntity.Genre>(x => x.Id == exampleGenre.Id),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleId),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException(
            $"Genre '{exampleId}' not found"
        ));
        var useCase = new UseCase.DeleteGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = new UseCase.DeleteGenreInput(
            exampleId
        );

        var action = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found");
        genreRepositoryMock.Verify(
            x => x.Get(
                It.Is<Guid>(x => x == exampleId),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
