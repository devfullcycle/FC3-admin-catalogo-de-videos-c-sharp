using FC.Codeflix.Catalog.Domain.Enum;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
public class CastMemberModelOutput
{
    public CastMemberModelOutput(
        Guid id, 
        string name, 
        CastMemberType type, 
        DateTime createdAt
    )
    {
        Id = id;
        Name = name;
        Type = type;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public CastMemberType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static CastMemberModelOutput FromCastMember(DomainEntity.CastMember castMember)
        => new(
            castMember.Id,
            castMember.Name,
            castMember.Type,
            castMember.CreatedAt
        );
}
