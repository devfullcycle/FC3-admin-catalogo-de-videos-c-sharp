using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object, unitOfWorkMock.Object
        );
        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            uow => uow.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateAggregate))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(nameof(GetInvalidInputs))]
    public async void ThrowWhenCantInstantiateAggregate(
        CreateCategoryInput input,
        string exceptionMessage
    )
    {
        var useCase = new UseCases.CreateCategory(
            _fixture.GetRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object
        );

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }

    private static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();

        // nome não pode ser menor que 3 caracteres
        var invalidInputShortName = fixture.GetInput();
        invalidInputShortName.Name = 
            invalidInputShortName.Name.Substring(0, 2);
        invalidInputsList.Add(new object[] {
            invalidInputShortName,
            "Name should be at leats 3 characters long"
        });

        // nome não pode ser maior do que 255 caracteres
        var invalidInputTooLongName = fixture.GetInput();
        var tooLongNameForCategory = fixture.Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {fixture.Faker.Commerce.ProductName()}";
        invalidInputTooLongName.Name = tooLongNameForCategory;
        invalidInputsList.Add(new object[] {
            invalidInputTooLongName,
            "Name should be less or equal 255 characters long"
        });
  
        // description não pode ser nula
        // nome não pode ser null

        return invalidInputsList;
    }
}
