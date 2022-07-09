using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.ListCastMembers;

[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTestFixtureCollection
    : ICollectionFixture<ListCastMembersTestFixture>
{ }

public class ListCastMembersTestFixture
    : CastMemberUseCasesBaseFixture
{
    public List<DomainEntity.CastMember> GetExampleCastMembersList(int quantity)
        => Enumerable
            .Range(1, quantity)
            .Select(_ => GetExampleCastMember())
            .ToList();
}
