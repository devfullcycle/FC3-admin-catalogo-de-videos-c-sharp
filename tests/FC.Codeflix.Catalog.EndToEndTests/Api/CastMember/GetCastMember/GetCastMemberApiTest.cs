using System;
using System.Net;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.GetCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class GetCastMemberApiTest
{
    private readonly CastMemberApiBaseFixture _fixture;

    public GetCastMemberApiTest(CastMemberApiBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Get))]
    [Trait("EndToEnd/API", "CatMembers/Get - EndPoints")]
    public async Task Get()
    {
        var examples = _fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        await _fixture.Persistence.InsertList(examples);

        var (response, output) =
            await _fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>(
                $"castmembers/{example.Id.ToString()}"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "CatMembers/Get - EndPoints")]
    public async Task NotFound()
    {
        await _fixture.Persistence.InsertList(
            _fixture.GetExampleCastMembersList(5)
        );
        var randomGuid = Guid.NewGuid();

        var (response, output) =
            await _fixture.ApiClient.Get<ProblemDetails>(
                $"castmembers/{randomGuid.ToString()}"
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output!.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
    }
}
