using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
public interface IDeleteCastMember
    : IRequestHandler<DeleteCastMemberInput>
{ }
