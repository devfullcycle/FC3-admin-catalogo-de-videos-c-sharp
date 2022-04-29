using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenresInput :
    PaginatedListInput, IRequest<ListGenresOutput>
{
    public ListGenresInput(
        int page, 
        int perPage, 
        string search, 
        string sort, 
        SearchOrder dir
    ) 
        : base(page, perPage, search, sort, dir)
    {}
}
