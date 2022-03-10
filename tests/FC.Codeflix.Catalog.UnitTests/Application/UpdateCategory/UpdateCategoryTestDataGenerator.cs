using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;
public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for(int indice = 0; indice < times; indice++)
        {
            var exampleCategory = fixture.GetExampleCategory();
            var exampleInput = new UpdateCategoryInput(
                exampleCategory.Id,
                fixture.GetValidCategoryName(),
                fixture.GetValidCategoryDescription(),
                fixture.getRandomBoolean()
            );
            yield return new object[] {
                exampleCategory, exampleInput
            };
        }
    }
}
