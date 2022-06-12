using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Api.ApiModels.Genre;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest
{
    private readonly UpdateGenreApiTestFixture _fixture;

    public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/Api", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenre()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(
            _fixture.GetValidGenreName(), 
            _fixture.GetRandomBoolean()
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>(
                $"/genres/{targetGenre.Id}",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive);
        var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);
    }
}
