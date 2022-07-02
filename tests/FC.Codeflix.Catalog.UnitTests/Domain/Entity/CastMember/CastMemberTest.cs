using Xunit;
using Entity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentAssertions;
using System;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest
{
    private CastMemberTestFixture _fixture;

    public CastMemberTest(CastMemberTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        var datetimeBefore = DateTime.Now.AddSeconds(-1);
        var name = "Jorge Lucas";
        var type = CastMemberType.Director;

        var castMember = new Entity.CastMember(name, type);

        var datetimeAfter = DateTime.Now.AddSeconds(1);
        castMember.Id.Should().NotBeNull();
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        (castMember.CreatedAt >= datetimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= datetimeAfter).Should().BeTrue();
    }
}
