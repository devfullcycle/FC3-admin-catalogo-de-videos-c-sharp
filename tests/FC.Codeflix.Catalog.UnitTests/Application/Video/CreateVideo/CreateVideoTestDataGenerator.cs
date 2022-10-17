using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;
public class CreateVideoTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var fixture = new CreateVideoTestFixture();
        var invalidInputsList = new List<object[]>();
        const int totalInvalidCases = 2;

        for (int index = 0; index < times; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[] {
                        new CreateVideoInput(
                            "",
                            fixture.GetValidDescription(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating()
                        ),
                        "'Title' is required"
                    });
                    break;
                case 1:
                    invalidInputsList.Add(new object[] {
                        new CreateVideoInput(
                            fixture.GetValidTitle(),
                            "",
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating()
                        ),
                        "'Description' is required"
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputsList;
    }
}
