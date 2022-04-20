using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest (GenreTestFixture fixture)
        => _fixture = fixture;
         
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genreName = _fixture.GetValidName();

        var datetimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(genreName);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt >= datetimeBefore).Should().BeTrue();
        (genre.CreatedAt <= datetimeAfter).Should().BeTrue();
    }


    [Theory(DisplayName = nameof(InstantiateThrowWhenNameEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void InstantiateThrowWhenNameEmpty(string? name)
    {
        var action = 
            () => new DomainEntity.Genre(name!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var genreName = _fixture.GetValidName();

        var datetimeBefore = DateTime.Now;
        var genre = new DomainEntity.Genre(genreName, isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(genreName);
        genre.IsActive.Should().Be(isActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
        (genre.CreatedAt >= datetimeBefore).Should().BeTrue();
        (genre.CreatedAt <= datetimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);
        var oldName = genre.Name;

        genre.Activate();

        genre.Should().NotBeNull();
        genre.Name.Should().Be(oldName);
        genre.IsActive.Should().BeTrue();
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive);
        var oldName = genre.Name;

        genre.Deactivate();

        genre.Should().NotBeNull();
        genre.IsActive.Should().BeFalse();
        genre.Name.Should().Be(oldName);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {
        var genre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidName();
        var oldIsActive = genre.IsActive;

        genre.Update(newName);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(newName);
        genre.IsActive.Should().Be(oldIsActive);
        genre.CreatedAt.Should().NotBeSameDateAs(default);
    }


    [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void UpdateThrowWhenNameIsEmpty(string? name)
    {
        var genre = _fixture.GetExampleGenre();

        var action = 
            () => genre.Update(name!);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
}
