using FC.Codeflix.Catalog.Application.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
public class ListCastMembersOutput : PaginatedListOutput<CastMemberModelOutput>
{
    public ListCastMembersOutput(int page, int perPage, int total, IReadOnlyList<CastMemberModelOutput> items) 
        : base(page, perPage, total, items)
    { }

    public static ListCastMembersOutput FromSearchOutput(
        SearchOutput<DomainEntity.CastMember> searchOutput
    ) => new(
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
