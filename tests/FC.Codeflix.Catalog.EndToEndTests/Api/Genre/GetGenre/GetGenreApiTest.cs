using System.Collections.Generic;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenre;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest
{
    private GetGenreApiTestFixture _fixture;

    public GetGenreApiTest(GetGenreApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task GetGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
    }
}
