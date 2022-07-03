using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
public interface IGetCastMember
    : IRequestHandler<GetCastMemberInput, CastMemberModelOutput>
{}
