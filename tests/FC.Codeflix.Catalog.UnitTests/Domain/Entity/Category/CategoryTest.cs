using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

public class CategoryTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // Arrange
        var validData = new
        {
            Name = "category name",
            Description = "category description"
        };

        // Act
        var category = new Category(validData.Name, validData.Description);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(validData.Name, category.Name);
        Assert.Equal(validData.Description, category.Description);
    }
}
