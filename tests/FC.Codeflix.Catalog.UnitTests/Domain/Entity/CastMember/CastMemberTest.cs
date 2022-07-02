using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentAssertions;
using System;
using FC.Codeflix.Catalog.Domain.Exceptions;

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
        var name = _fixture.GetValidName();
        var type = _fixture.GetRandomCastMemberType();

        var castMember = new DomainEntity.CastMember(name, type);

        var datetimeAfter = DateTime.Now.AddSeconds(1);
        castMember.Id.Should().NotBe(default(Guid));
        castMember.Name.Should().Be(name);
        castMember.Type.Should().Be(type);
        (castMember.CreatedAt >= datetimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= datetimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowErrorWhenNameIsInvalid(string? name)
    {
        var type = _fixture.GetRandomCastMemberType();

        var action = () => new DomainEntity.CastMember(name!, type);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        var castMember = _fixture.GetExampleCastMember(); ;

        castMember.Update(newName, newType);

        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
    }

    [Theory(DisplayName = nameof(UpdateThrowsErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateThrowsErrorWhenNameIsInvalid(string? newName)
    {
        var newType = _fixture.GetRandomCastMemberType();
        var castMember = _fixture.GetExampleCastMember(); ;

        var action = () => castMember.Update(newName!, newType);

        action.Should().Throw<EntityValidationException>()
            .WithMessage($"Name should not be empty or null");
    }
}
