using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembers : IListCastMembers
{
    private readonly ICastMemberRepository _repository;

    public ListCastMembers(ICastMemberRepository repository) 
        => _repository = repository;

    public async Task<ListCastMembersOutput> Handle(
        ListCastMembersInput request, 
        CancellationToken cancellationToken
    )
    {
        var searchOutput = await _repository.Search(request.ToSearchInput(), cancellationToken);
        return ListCastMembersOutput.FromSearchOutput(searchOutput);
    }
}
