using FC.Codeflix.Catalog.UnitTests.Common;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Domain.ValueObject;

public class ImageTest : BaseFixture
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Image - ValueObjects")]
    public void Instantiate()
    {
        var path = Faker.Image.PicsumUrl();

        var image = new Image(path);

        image.Path.Should().Be(path);
    }

    [Fact(DisplayName = nameof(EqualsByPath))]
    [Trait("Domain", "Image - ValueObjects")]
    public void EqualsByPath()
    {
        var path = Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(path);

        var isItEquals = image == sameImage;

        isItEquals.Should().BeTrue();
    }
    
    [Fact(DisplayName = nameof(DifferentByPath))]
    [Trait("Domain", "Image - ValueObjects")]
    public void DifferentByPath()
    {
        var path = Faker.Image.PicsumUrl();
        var differentPath = Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(differentPath);

        var isItDifferent = image != sameImage;

        isItDifferent.Should().BeTrue();
    }


}
