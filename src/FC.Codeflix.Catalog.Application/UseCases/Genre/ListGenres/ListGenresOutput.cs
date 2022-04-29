using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenresOutput
    : PaginatedListOutput<GenreModelOutput>
{
    public ListGenresOutput(
        int page, 
        int perPage, 
        int total, 
        IReadOnlyList<GenreModelOutput> items
    ) 
        : base(page, perPage, total, items)
    {}
}
