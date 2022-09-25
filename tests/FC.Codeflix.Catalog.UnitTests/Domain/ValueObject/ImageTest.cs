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
}
