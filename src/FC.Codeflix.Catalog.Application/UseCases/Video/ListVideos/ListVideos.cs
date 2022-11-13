using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using System.Runtime.CompilerServices;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;

    public ListVideos(IVideoRepository videoRepository) 
        => _videoRepository = videoRepository;

    public async Task<ListVideosOutput> Handle(
        ListVideosInput input, 
        CancellationToken cancellationToken)
    {
        var result = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);
        return new ListVideosOutput(
            result.CurrentPage, 
            result.PerPage,
            result.Total,
            result.Items.Select(VideoModelOutput.FromVideo).ToList());
    }
}
