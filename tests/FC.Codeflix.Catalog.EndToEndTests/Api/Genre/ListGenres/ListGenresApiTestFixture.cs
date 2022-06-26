using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenres;

[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCiollection
    : ICollectionFixture<ListGenresApiTestFixture>
{ }

public class ListGenresApiTestFixture
    : GenreBaseFixture
{
    public List<DomainEntity.Genre> GetExampleListGenresByNames(List<string> names)
        => names
            .Select(name => GetExampleGenre(name: name))
            .ToList();
}
