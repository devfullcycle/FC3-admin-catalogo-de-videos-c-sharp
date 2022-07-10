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
        var searchOutput = await _repository.Search(
            new SearchInput(
                request.Page, 
                request.PerPage, 
                request.Search, 
                request.Sort, 
                request.Dir
            ), 
            cancellationToken
        );
        return new ListCastMembersOutput(
            searchOutput.CurrentPage, 
            searchOutput.PerPage, 
            searchOutput.Total, 
            searchOutput.Items
                .Select(castmember 
                    => CastMemberModelOutput.FromCastMember(castmember))
                .ToList()
                .AsReadOnly()
        );
    }
}
