using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTestFixtureCollection
    : ICollectionFixture<DeleteCategoryTestFixture>
{ }

public class DeleteCategoryTestFixture
    : CategoryUseCasesBaseFixture
{ }
