using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenres;

[Collection(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTest
{
    private readonly ListGenresApiTestFixture _fixture;

    public ListGenresApiTest(ListGenresApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("EndToEnd/Api", "Genre/ListGenres - Endpoints")]
    public async Task List()
    {
        List<DomainEntity.Genre> exampleGenres = _fixture.GetExampleListGenres(10);
        await _fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleGenres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data!.Count.Should().Be(exampleGenres.Count);
        output.Data.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMillisseconds()
                .Should().Be(exampleItem.CreatedAt.TrimMillisseconds());
        });
    }
}
