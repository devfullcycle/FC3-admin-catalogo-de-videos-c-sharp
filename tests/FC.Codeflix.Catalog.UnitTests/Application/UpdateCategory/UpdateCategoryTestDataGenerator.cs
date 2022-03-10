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
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);
            yield return new object[] {
                exampleCategory, exampleInput
            };
        }
    }
}
