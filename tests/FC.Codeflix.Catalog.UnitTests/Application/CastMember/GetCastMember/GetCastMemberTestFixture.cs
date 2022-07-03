using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.GetCastMember;

[CollectionDefinition(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTestFixtureCollection
    : ICollectionFixture<GetCastMemberTestFixture>
{ }

public class GetCastMemberTestFixture
    : CastMemberUseCasesBaseFixture
{ }
