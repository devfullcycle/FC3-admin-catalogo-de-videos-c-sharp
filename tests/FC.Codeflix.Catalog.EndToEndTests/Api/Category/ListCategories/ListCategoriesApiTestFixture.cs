using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTestFixtureCollection
    : ICollectionFixture<ListCategoriesApiTestFixture>
{ }

public class ListCategoriesApiTestFixture
    : CategoryBaseFixture
{ }
