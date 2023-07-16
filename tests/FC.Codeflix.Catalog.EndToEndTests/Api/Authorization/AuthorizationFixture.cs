using FC.Codeflix.Catalog.EndToEndTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Authorization;

[CollectionDefinition(nameof(AuthorizationFixture))]
public class AuthorizationFixtureCollection
    : ICollectionFixture<AuthorizationFixture>
{
}

public class AuthorizationFixture : BaseFixture
{
}
