using System;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.GetCastMember;

[Collection(nameof(CastMemberUseCasesBaseFixture))]
public class GetCastMemberTest
{
    private readonly CastMemberUseCasesBaseFixture _fixture;

    public GetCastMemberTest(CastMemberUseCasesBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    public async Task GetCastMember()
    {
        var examples = _fixture.GetExampleCastMembersList(10);
        var example = examples[5];
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var useCase = new UseCase.GetCastMember(
            new CastMemberRepository(_fixture.CreateDbContext(true))
        );
        var input = new UseCase.GetCastMemberInput(example.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(example.Name);
        output.Type.Should().Be(example.Type);
        output.Id.Should().Be(example.Id);
    }


    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Application", "GetCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var useCase = new UseCase.GetCastMember(
            new CastMemberRepository(_fixture.CreateDbContext())
        );
        var randomGuid = Guid.NewGuid();
        var input = new UseCase.GetCastMemberInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");
    }
}
