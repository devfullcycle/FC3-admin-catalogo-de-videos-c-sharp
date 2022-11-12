using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.GetVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Get))]
    [Trait("Application", "GetVideo - Use Cases")]
    public void Get()
    {

    }
}
