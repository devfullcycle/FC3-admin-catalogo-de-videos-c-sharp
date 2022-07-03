using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.DeleteCastMember;

[CollectionDefinition(nameof(DeleteCastMemberFixture))]
public class DeleteCastMemberFixtureCollection
    : ICollectionFixture<DeleteCastMemberFixture>
{ }

public class DeleteCastMemberFixture
    : CastMemberUseCasesBaseFixture
{ }
