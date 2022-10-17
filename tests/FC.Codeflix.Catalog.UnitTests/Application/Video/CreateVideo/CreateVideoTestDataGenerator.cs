using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using Moq;
using System.Collections;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;
public class CreateVideoTestDataGenerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var fixture = new CreateVideoTestFixture();
        var invalidInputsList = new List<object[]>();
        const int totalInvalidCases = 4;

        for (int index = 0; index < totalInvalidCases * 2; index++)
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
                case 2:
                    invalidInputsList.Add(new object[] {
                        new CreateVideoInput(
                            fixture.GetTooLongTitle(),
                            fixture.GetValidDescription(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating()
                        ),
                        "'Title' should be less or equal 255 characters long"
                    });
                    break;
                case 3:
                    invalidInputsList.Add(new object[] {
                        new CreateVideoInput(
                            fixture.GetValidTitle(),
                            fixture.GetTooLongDescription(),
                            fixture.GetValidYearLaunched(),
                            fixture.GetRandomBoolean(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating()
                        ),
                        "'Description' should be less or equal 4000 characters long"
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputsList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
