using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.ListVideos;

public class ListVideosTest
{
    private readonly ListVideosTestFixture _fixture;

    public ListVideosTest(ListVideosTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListVideos))]
    [Trait("Application", "ListVideos - Use Cases")]
    public void ListVideos()
    {

    }
}
