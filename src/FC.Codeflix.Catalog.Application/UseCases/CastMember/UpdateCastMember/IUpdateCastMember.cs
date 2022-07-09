using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
public interface IUpdateCastMember 
    : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
}
