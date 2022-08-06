using System.Net;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

[Collection(nameof(CastMemberApiBaseFixture))]
public class CreateCastMemberApiTest
{
    private readonly CastMemberApiBaseFixture _fixture;

    public CreateCastMemberApiTest(CastMemberApiBaseFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Create))]
    [Trait("End2End/Api", "CastMembers/Create")]
    public async Task Create()
    {
        var example = _fixture.GetExampleCastMember();

        var (response, output) =
            await _fixture.ApiClient.Post<TestApiResponse<CastMemberModelOutput>>(
                "/castmembers",
                new CreateCastMemberInput(example.Name, example.Type)
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data!.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);
        var castMemberInDb = await _fixture.Persistence.GetById(output.Data.Id);
        castMemberInDb.Should().NotBeNull();
        castMemberInDb!.Name.Should().Be(example.Name);
        castMemberInDb.Type.Should().Be(example.Type);
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsEmpty))]
    [Trait("End2End/Api", "CastMembers/Create")]
    public async Task ThrowWhenNameIsEmpty()
    {
        var example = _fixture.GetExampleCastMember();

        var (response, output) =
            await _fixture.ApiClient.Post<ProblemDetails>(
                "/castmembers",
                new CreateCastMemberInput("", example.Type)
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors ocurred");
        output.Detail.Should().Be("Name should not be empty or null");
    }
}
