using System.Net;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class UpdatCastMemberApiTest
{
    private readonly CastMemberApiBaseFixture _fixture;

    public UpdatCastMemberApiTest(CastMemberApiBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Update))]
    [Trait("EndToEnd/API", "CastMembers/Update")]
    public async Task Update()
    {
        var examples = _fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        await _fixture.Persistence.InsertList(examples);

        var (response, output) =
            await _fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>(
                $"castmembers/{example.Id.ToString()}",
                new UpdateCastMemberInput(example.Id, newName, newType)
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(newName);
        output.Data.Type.Should().Be(newType);
        var castMemberFromDb = await _fixture.Persistence.GetById(example.Id);
        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb!.Id.Should().Be(example.Id);
        castMemberFromDb.Name.Should().Be(newName);
        castMemberFromDb.Type.Should().Be(newType);
    }
}
