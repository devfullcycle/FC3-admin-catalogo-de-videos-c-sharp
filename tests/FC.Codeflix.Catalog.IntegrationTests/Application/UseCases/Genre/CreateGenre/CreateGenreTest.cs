using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using System;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Integration/Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        CreateGenreInput input = _fixture.GetExampleInput();
        CodeflixCatalogDbContext actDbContext = _fixture.CreateDbContext();
        UseCase.CreateGenre createGenre = new UseCase.CreateGenre(
            new GenreRepository(actDbContext),
            new UnitOfWork(actDbContext),
            new CategoryRepository(actDbContext)
        );

        GenreModelOutput output = await createGenre.Handle(
            input, 
            CancellationToken.None
        );

        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Categories.Should().HaveCount(0);
        CodeflixCatalogDbContext assertDbContext = _fixture.CreateDbContext(true);
        DomainEntity.Genre? genreFromDb = 
            await assertDbContext.Genres.FindAsync(output.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
    }
}
