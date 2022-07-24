using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus.DataSets;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Integration/Application", "CreateCastMember - Use Cases")]
    public async Task CreatCastMember()
    {
        var actDbContext = _fixture.CreateDbContext();
        var repository = new CastMemberRepository(actDbContext);
        var unitOfWork = new UnitOfWork(actDbContext);
        var useCase = new UseCase.CreateCastMember(repository, unitOfWork);
        var input = new UseCase.CreateCastMemberInput(
            _fixture.GetValidName(),
            _fixture.GetRandomCastMemberType()
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        var assertDbContext = _fixture.CreateDbContext(true);
        var castMembers = await assertDbContext.CastMembers.AsNoTracking().ToListAsync();
        castMembers.Should().HaveCount(1);
        var castMemberFromDb = castMembers[0];
        castMemberFromDb.Name.Should().Be(input.Name);
        castMemberFromDb.Type.Should().Be(input.Type);
        castMemberFromDb.Id.Should().Be(output.Id);
    }
}
