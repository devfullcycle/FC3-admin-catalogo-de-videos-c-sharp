using System;
using System.Net;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMembers;

[Collection(nameof(CastMemberApiBaseFixture))]
public class ListCastMembersApiTest : IDisposable
{
    private readonly CastMemberApiBaseFixture _fixture;

    public ListCastMembersApiTest(CastMemberApiBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("EndToEnd/API", "CastMembers/List")]
    public async Task List()
    {
        var examples = _fixture.GetExampleCastMembersList(5);
        await _fixture.Persistence.InsertList(examples);

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
                "castmembers"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(examples.Count);
        output.Data!.Should().HaveCount(examples.Count);
        output.Data!.ForEach(outputItem =>
        {
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
        });
    }

    [Theory(DisplayName = nameof(Paginated))]
    [Trait("EndToEnd/API", "CastMembers/List")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task Paginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var examples = _fixture.GetExampleCastMembersList(quantityToGenerate);
        await _fixture.Persistence.InsertList(examples);

        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
                "castmembers", new ListCastMembersInput() { Page = page, PerPage = perPage}
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(examples.Count);
        output.Data!.Should().HaveCount(expectedQuantityItems);
        output.Data!.ForEach(outputItem =>
        {
            var exampleItem = examples.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
        });
    }

    [Fact(DisplayName = nameof(ReturnsEmptyWhenEmpty))]
    [Trait("EndToEnd/API", "CastMembers/List")]
    public async Task ReturnsEmptyWhenEmpty()
    {
        var (response, output) =
            await _fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>(
                "castmembers"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(0);
        output.Data!.Should().HaveCount(0);
    }

    public void Dispose() => _fixture.CleanPersistence();
}
