
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestApiFixture))]
public class DeleteGenreTestApiFixtureCollection
    : ICollectionFixture<DeleteGenreTestApiFixture>
{ }

public class DeleteGenreTestApiFixture
    : GenreBaseFixture
{
}
