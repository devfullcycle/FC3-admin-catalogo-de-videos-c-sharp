using System.Threading.Tasks;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture _fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            _fixture.GetCategoryRepositoryMock().Object
        );
        var input = new UseCase.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
            newIsActive
        );

        GenreModelOutput output = 
            await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(0);
        genreRepositoryMock.Verify(
            x => x.Update(
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
}
