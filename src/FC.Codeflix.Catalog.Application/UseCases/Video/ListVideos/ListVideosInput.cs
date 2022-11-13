using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideosInput : PaginatedListInput, IRequest<ListVideosOutput>
{
    public ListVideosInput(
        int page, 
        int perPage, 
        string search, 
        string sort, 
        SearchOrder dir) 
        : base(page, perPage, search, sort, dir)
    { }
}
