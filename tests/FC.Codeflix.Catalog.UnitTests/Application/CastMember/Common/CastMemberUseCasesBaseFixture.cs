using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common;
using System;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
public class CastMemberUseCasesBaseFixture
    : BaseFixture
{
    public string GetValidName()
        => Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType()
        => (CastMemberType)(new Random()).Next(1, 2);
}
