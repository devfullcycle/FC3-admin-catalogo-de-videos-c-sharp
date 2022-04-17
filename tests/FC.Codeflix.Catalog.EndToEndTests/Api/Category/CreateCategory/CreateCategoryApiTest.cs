using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FC.Codeflix.Catalog.Api.ApiModels.Response;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
    : IDisposable
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    public async Task CreateCategory()
    {
        var input = _fixture.getExampleInput();

        var (response, output) = await _fixture.
            ApiClient.Post<ApiResponse<CategoryModelOutput>>(
                "/categories",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should()
            .NotBeSameDateAs(default);
        var dbCategory = await _fixture
            .Persistence.GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.Id.Should().NotBeEmpty();
        dbCategory.CreatedAt.Should()
            .NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
    [Trait("EndToEnd/API", "Category/Create - Endpoints")]
    [MemberData(
        nameof(CreateCategoryApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateCategoryApiTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstantiateAggregate(
        CreateCategoryInput input,
        string expectedDetail
    ){
        var (response, output) = await _fixture.
            ApiClient.Post<ProblemDetails>(
                "/categories",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors ocurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }
    public void Dispose()
        => _fixture.CleanPersistence();
}
