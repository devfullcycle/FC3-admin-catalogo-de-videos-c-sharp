using System.Collections.Generic;
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
    
    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var namesToGenerate = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        };
        var examples = _fixture.GetExampleCastMembersListByNames(namesToGenerate);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(_fixture.CreateDbContext(true));
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new UseCase.ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = examples.First(example => example.Id == outputItem.Id);
            exampleItem.Should().BeEquivalentTo(outputItem);
        });
    }

    [Theory(DisplayName = nameof(Ordering))]
    [Trait("Integration/Application", "ListCastMembers - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task Ordering(
        string orderBy,
        string order
    )
    {
        var examples = _fixture.GetExampleCastMembersList(10);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();
        var repository = new CastMemberRepository(_fixture.CreateDbContext(true));
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var useCase = new UseCase.ListCastMembers(repository);
        var input = new UseCase.ListCastMembersInput(1, 20, "", orderBy, searchOrder);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Items.Should().HaveCount(examples.Count());
        output.Total.Should().Be(examples.Count());
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        var orderedList = _fixture.CloneListOrdered(
            examples, 
            orderBy, searchOrder
        );
        for (int i = 0; i < orderedList.Count(); i++)
            output.Items[i].Should().BeEquivalentTo(orderedList[i]);
    }
}
