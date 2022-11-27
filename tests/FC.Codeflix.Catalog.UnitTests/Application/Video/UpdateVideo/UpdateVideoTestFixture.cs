using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;

[CollectionDefinition(nameof(UpdateVideoTestFixture))]
public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
{ }

public class UpdateVideoTestFixture : VideoTestFixtureBase
{ }
