using System.Threading;
using System.Threading.Tasks;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System;
using System.Linq;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private readonly CastMemberRepositoryTestFixture _fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Insert()
    {
        var castMemberExample = _fixture.GetExampleCastMember();
        var context = _fixture.CreateDbContext();
        var repository = new Repository.CastMemberRepository(context);

        await repository.Insert(castMemberExample, CancellationToken.None);
        context.SaveChanges();

        var assertionContext = _fixture.CreateDbContext(true);
        var castMemberFromDb = await assertionContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Name.Should().Be(castMemberExample.Name);
        castMemberFromDb.Type.Should().Be(castMemberExample.Type);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Get()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var repository = new Repository
            .CastMemberRepository(_fixture.CreateDbContext(true));

        var itemFromRepository = await repository.Get(
            castMemberExample.Id,
            CancellationToken.None
        );

        itemFromRepository.Should().NotBeNull();
        itemFromRepository!.Name.Should().Be(castMemberExample.Name);
        itemFromRepository.Type.Should().Be(castMemberExample.Type);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        var randomGuid = Guid.NewGuid();
        var repository = new Repository
            .CastMemberRepository(_fixture.CreateDbContext());

        var action = async () => await repository.Get(
            randomGuid,
            CancellationToken.None
        );

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Delete()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository
            .CastMemberRepository(actDbContext);

        await repository.Delete(
            castMemberExample, CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertionContext = _fixture.CreateDbContext(true);
        var itemsInDatabase = assertionContext.CastMembers
            .AsNoTracking()
            .ToList();
        itemsInDatabase.Should().HaveCount(4);
        itemsInDatabase.Should().NotContain(castMemberExample);
    }
    
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Update()
    {
        var castMemberExampleList = _fixture.GetExampleCastMembersList(5);
        var castMemberExample = castMemberExampleList[3];
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var arrangeContext = _fixture.CreateDbContext();
        await arrangeContext.AddRangeAsync(castMemberExampleList);
        await arrangeContext.SaveChangesAsync();
        var actDbContext = _fixture.CreateDbContext(true);
        var repository = new Repository
            .CastMemberRepository(actDbContext);

        castMemberExample.Update(newName, newType);
        await repository.Update(
            castMemberExample, CancellationToken.None
        );
        await actDbContext.SaveChangesAsync();

        var assertionContext = _fixture.CreateDbContext(true);
        var castMemberDb = await assertionContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);
        castMemberDb.Should().NotBeNull();
        castMemberDb!.Name.Should().Be(newName);
        castMemberDb.Type.Should().Be(newType);
    }

    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Search()
    {
        var exampleList = _fixture.GetExampleCastMembersList(10);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleList);
        await arrangeDbContext.SaveChangesAsync();
        var castMembersRepository = new Repository
            .CastMemberRepository(_fixture.CreateDbContext(true));

        var searchResult = await castMembersRepository.Search(
            new SearchInput(1, 20, "", "", SearchOrder.Asc),
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(20);
        searchResult.Total.Should().Be(10);
        searchResult.Items.Should().HaveCount(10);
        searchResult.Items.ToList().ForEach(resultItem =>
        {
            var example = exampleList.Find(x => x.Id == resultItem.Id);
            example.Should().NotBeNull();
            resultItem.Name.Should().Be(example!.Name);
            resultItem.Type.Should().Be(example.Type);
        });
    }
    
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var castMembersRepository = new Repository
            .CastMemberRepository(_fixture.CreateDbContext());

        var searchResult = await castMembersRepository.Search(
            new SearchInput(1, 20, "", "", SearchOrder.Asc),
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(1);
        searchResult.PerPage.Should().Be(20);
        searchResult.Total.Should().Be(0);
        searchResult.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchWithPagination))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchWithPagination(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleList = _fixture.GetExampleCastMembersList(quantityToGenerate);
        var arrangeDbContext = _fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleList);
        await arrangeDbContext.SaveChangesAsync();
        var castMembersRepository = new Repository
            .CastMemberRepository(_fixture.CreateDbContext(true));

        var searchResult = await castMembersRepository.Search(
            new SearchInput(page, perPage, "", "", SearchOrder.Asc),
            CancellationToken.None
        );

        searchResult.Should().NotBeNull();
        searchResult.CurrentPage.Should().Be(page);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.Total.Should().Be(quantityToGenerate);
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        searchResult.Items.ToList().ForEach(resultItem =>
        {
            var example = exampleList.Find(x => x.Id == resultItem.Id);
            example.Should().NotBeNull();
            resultItem.Name.Should().Be(example!.Name);
            resultItem.Type.Should().Be(example.Type);
        });
    }
}
