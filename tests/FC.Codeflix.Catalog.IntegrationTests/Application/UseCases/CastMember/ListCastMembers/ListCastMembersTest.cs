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
    
    [Fact(DisplayName = nameof(Empty))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    public async Task Empty()
    {
        var repository = new CastMemberRepository(_fixture.CreateDbContext());
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new UseCase.ListCastMembersInput(1, 10, "", "", SearchOrder.Asc);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(0);
        output.Total.Should().Be(0);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
    }

    [Theory(DisplayName = nameof(Pagination))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task Pagination(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var examples = _fixture.GetExampleCastMembersList(quantityToGenerate);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new UseCase.ListCastMembersInput(page, perPage, "", "", SearchOrder.Asc);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Total.Should().Be(quantityToGenerate);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.First(example => example.Id == outputItem.Id);
            exampleItem.Should().BeEquivalentTo(outputItem);
        });
    }
}
