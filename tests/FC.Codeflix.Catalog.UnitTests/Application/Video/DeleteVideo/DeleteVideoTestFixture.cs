using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.DeleteVideo;

[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection
    : ICollectionFixture<DeleteVideoTestFixture>
{ }

public class DeleteVideoTestFixture : VideoTestFixtureBase
{
}
