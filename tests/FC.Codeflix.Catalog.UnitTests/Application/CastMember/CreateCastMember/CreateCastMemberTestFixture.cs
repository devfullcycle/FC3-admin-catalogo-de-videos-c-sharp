using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection
    : ICollectionFixture<CreateCastMemberTestFixture>
{ }

public class CreateCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{
}
