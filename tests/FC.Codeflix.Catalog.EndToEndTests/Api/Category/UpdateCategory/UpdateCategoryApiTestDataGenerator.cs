using System.Collections.Generic;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;
public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateCategoryApiTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < totalInvalidCases; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    var input1 = fixture.GetExampleInput();
                    input1.Name = fixture.GetInvalidNameTooShort();
                    invalidInputsList.Add(new object[] {
                        input1,
                        "Name should be at least 3 characters long"
                    });
                    break;
                case 1:
                    var input2 = fixture.GetExampleInput();
                    input2.Name = fixture.GetInvalidNameTooLong();
                    invalidInputsList.Add(new object[] {
                        input2,
                        "Name should be less or equal 255 characters long"
                    });
                    break;
                case 2:
                    var input3 = fixture.GetExampleInput();
                    input3.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputsList.Add(new object[] {
                        input3,
                        "Description should be less or equal 10000 characters long"
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputsList;
    }
}
