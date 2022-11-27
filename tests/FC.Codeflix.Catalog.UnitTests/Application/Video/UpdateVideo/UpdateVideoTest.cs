using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[Collection(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideoTestFixture _fixture;

    public UpdateVideoTest(UpdateVideoTestFixture fixture) 
        => _fixture = fixture;

    [Fact]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public void UpdateVideo()
    {

    }
}
