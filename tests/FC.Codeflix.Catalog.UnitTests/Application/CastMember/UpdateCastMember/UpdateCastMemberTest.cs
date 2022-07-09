using System;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Update))]
    [Trait("Application", "UpdateCastMember - UseCases")]
    public async Task Update()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMemberExample = _fixture.GetExampleCastMember();
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetRandomCastMemberType();
        repositoryMock
            .Setup(x => x.Get(
                It.Is<Guid>(x => x == castMemberExample.Id), 
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(castMemberExample);
        var input = new UseCase.UpdateCastMemberInput(
            castMemberExample.Id,
            newName,
            newType
        );
        var useCase = new UseCase.UpdateCastMember(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        CastMemberModelOutput output = await useCase.Handle(
            input, 
            CancellationToken.None
        );

        output.Id.Should().Be(castMemberExample.Id);
        output.Name.Should().Be(newName);
        output.Type.Should().Be(newType);

        repositoryMock
            .Verify(x => x.Get(
                It.Is<Guid>(x => x == input.Id),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        repositoryMock
            .Verify(x => x.Update(
                It.Is<DomainEntity.CastMember>(
                    x => (
                        x.Id == castMemberExample.Id && 
                        x.Name == input.Name && 
                        x.Type == input.Type
                    ) 
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        unitOfWorkMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>())
            , Times.Once
        );

    }
}
