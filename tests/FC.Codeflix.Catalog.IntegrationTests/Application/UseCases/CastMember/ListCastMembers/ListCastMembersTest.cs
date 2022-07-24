using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.ListCastMembers;

[Collection(nameof(CastMemberUseCasesBaseFixture))]
public class ListCastMembersTest
{
    private readonly CastMemberUseCasesBaseFixture _fixture;
    public ListCastMembersTest(CastMemberUseCasesBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(SimpleList))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    public async Task SimpleList()
    {
        var examples = _fixture.GetExampleCastMembersList(10);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new UseCase.ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(examples.Count);
        output.Total.Should().Be(examples.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.First(example => example.Id == outputItem.Id);
            exampleItem.Should().BeEquivalentTo(outputItem);
        });
    }
}
