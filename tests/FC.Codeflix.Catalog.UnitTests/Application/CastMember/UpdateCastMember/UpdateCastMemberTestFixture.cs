using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

[CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTestFixtureCollection
    : ICollectionFixture<UpdateCastMemberTestFixture>
{ }

public class UpdateCastMemberTestFixture 
    : CastMemberUseCasesBaseFixture
{ }
