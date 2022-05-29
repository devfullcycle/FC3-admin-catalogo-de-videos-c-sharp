using System.Threading.Tasks;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using System.Collections.Generic;

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

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException(
            $"Genre '{exampleId}' not found."
        ));
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            _fixture.GetUnitOfWorkMock().Object,
            _fixture.GetCategoryRepositoryMock().Object
        );
        var input = new UseCase.UpdateGenreInput(
            exampleId,
            _fixture.GetValidGenreName(),
            true
        );

        var action = async () 
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre '{exampleId}' not found.");
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ThrowWhenNameIsInvalid(string? name)
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
            name!,
            newIsActive
        );

        var action = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage($"Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreOnlyName(bool isActive)
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(isActive: isActive);
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
            newNameExample
        );

        GenreModelOutput output =
            await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(isActive);
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

    [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreAddingCategoriesIds()
    {
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleCategoriesIdsList);
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCase.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
            newIsActive,
            exampleCategoriesIdsList
        );

        GenreModelOutput output =
            await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
        exampleCategoriesIdsList.ForEach(
            expectedId => output.Categories.Should().Contain(relation => relation.Id == expectedId)
        );
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

    [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreReplacingCategoriesIds()
    {
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: _fixture.GetRandomIdsList()
        );
        var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleCategoriesIdsList);
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCase.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
            newIsActive,
            exampleCategoriesIdsList
        );

        GenreModelOutput output =
            await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleGenre.Id);
        output.Name.Should().Be(newNameExample);
        output.IsActive.Should().Be(newIsActive);
        output.CreatedAt.Should().BeSameDateAs(exampleGenre.CreatedAt);
        output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
        exampleCategoriesIdsList.ForEach(
            expectedId => output.Categories.Should().Contain(relation => relation.Id == expectedId)
        );
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

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: _fixture.GetRandomIdsList()
        );
        var exampleNewCategoriesIdsList = _fixture.GetRandomIdsList(10);        
        var listReturnedByCategoryRepository =
            exampleNewCategoriesIdsList
                .GetRange(0, exampleNewCategoriesIdsList.Count - 2);
        
        var IdsNotReturnedByCategoryRepository = 
            exampleNewCategoriesIdsList
                .GetRange(exampleNewCategoriesIdsList.Count - 2, 2);
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
            It.IsAny<List<Guid>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(listReturnedByCategoryRepository);
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCase.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
            newIsActive,
            exampleNewCategoriesIdsList
        );

        var action = async () 
            => await useCase.Handle(input, CancellationToken.None);

        var notFoundIdsAsString = String.Join(
            ", ", 
            IdsNotReturnedByCategoryRepository
        );
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage(
            $"Related category id (or ids) not found: {notFoundIdsAsString}"
        );
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutCategoriesIds()
    {
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: exampleCategoriesIdsList
        );
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
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
        output.Categories.Should().HaveCount(exampleCategoriesIdsList.Count);
        exampleCategoriesIdsList.ForEach(
            expectedId => output.Categories.Should().Contain(relation => relation.Id == expectedId)
        );
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

    [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIdsList))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithEmptyCategoriesIdsList()
    {
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var exampleCategoriesIdsList = _fixture.GetRandomIdsList();
        var exampleGenre = _fixture.GetExampleGenre(
            categoriesIds: exampleCategoriesIdsList
        );
        var newNameExample = _fixture.GetValidGenreName();
        var newIsActive = !exampleGenre.IsActive;
        genreRepositoryMock.Setup(x => x.Get(
            It.Is<Guid>(x => x == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);
        var useCase = new UseCase.UpdateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );
        var input = new UseCase.UpdateGenreInput(
            exampleGenre.Id,
            newNameExample,
            newIsActive,
            new List<Guid>()
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
