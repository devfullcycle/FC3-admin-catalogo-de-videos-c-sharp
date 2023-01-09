using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[CollectionDefinition(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTestFixtureCollection 
    : ICollectionFixture<VideoRepositoryTestFixture>
{
}

public class VideoRepositoryTestFixture : BaseFixture
{
}
