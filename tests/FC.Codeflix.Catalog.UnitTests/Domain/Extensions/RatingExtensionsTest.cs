using System;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Extensions;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Extensions;
public class RatingExtensionsTest
{
    [Theory(DisplayName = nameof(StringToRating))]
    [Trait("Domain", "Rating - Extensions")]
    [InlineData("ER", Rating.ER)]
    [InlineData("L", Rating.L)]
    [InlineData("10", Rating.Rate10)]
    [InlineData("12", Rating.Rate12)]
    [InlineData("14", Rating.Rate14)]
    [InlineData("16", Rating.Rate16)]
    [InlineData("18", Rating.Rate18)]
    public void StringToRating(string enumString, Rating expectedRating) 
        => enumString.ToRating().Should().Be(expectedRating);

    [Fact(DisplayName = nameof(ThrowsExceptionWhenInvalidString))]
    [Trait("Domain", "Rating - Extensions")]
    public void ThrowsExceptionWhenInvalidString()
    {
        var action = () => "Invalid".ToRating();
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
