using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using System;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Create))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task Create()
    {
        var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new CreateGenre(
            genreRepositoryMock.Object,
            unitOfWorkMock.Object
        );
        var input = _fixture.GetExampleInput();

        var datetimeBefore = DateTime.Now;
        var output = await useCase
            .Handle(input, CancellationToken.None);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genreRepositoryMock.Verify(x => x.Insert(
            It.IsAny<DomainEntity.Genre>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);
        (output.CreatedAt >= datetimeBefore).Should().BeTrue();
        (output.CreatedAt <= datetimeAfter).Should().BeTrue();
    }

}
