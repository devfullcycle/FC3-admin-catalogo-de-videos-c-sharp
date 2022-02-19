using Bogus;
using Xunit;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using System;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;

public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    // nao ser null
    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        Action action = 
            () => DomainValidation.NotNull(value, "Value");
        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string value = null;
        Action action =
            () => DomainValidation.NotNull(value, "FieldName");
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("FieldName should not be null");
    }


    // nao ser null ou vazio
    // tamanho minimo
    // tamanho maximo
}
