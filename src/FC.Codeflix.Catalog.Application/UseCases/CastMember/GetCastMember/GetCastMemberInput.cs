using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
public class GetCastMemberInput
    : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; private set; }
    public GetCastMemberInput(Guid id) => Id = id;
}
