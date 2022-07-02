using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

public interface ICreateCastMember
    : IRequestHandler<CreateCastMemberInput, CastMemberModelOutput>
{
}
