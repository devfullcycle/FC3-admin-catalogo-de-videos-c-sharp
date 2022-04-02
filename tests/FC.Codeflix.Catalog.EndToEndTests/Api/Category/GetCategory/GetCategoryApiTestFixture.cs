using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategoryById;

[CollectionDefinition(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTestFixtureCollection
    : ICollectionFixture<GetCategoryApiTestFixture>
{ }

public class GetCategoryApiTestFixture
    : CategoryBaseFixture
{ }
