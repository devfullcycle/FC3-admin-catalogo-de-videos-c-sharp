using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.GetVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection 
    : ICollectionFixture<CreateVideoTestFixture>
{ }

public class CreateVideoTestFixture : VideoTestFixtureBase
{ }
