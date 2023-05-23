using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
public class VideoBaseFixture
    : GenreBaseFixture
{
    public readonly VideoPersistence VideoPersistence;
    public VideoBaseFixture() :base() {
        VideoPersistence = new VideoPersistence(DbContext);
    }
}
