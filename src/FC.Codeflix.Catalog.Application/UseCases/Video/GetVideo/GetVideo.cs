using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;

public class GetVideo : IGetVideo
{
    private readonly IVideoRepository _repository;

    public GetVideo(IVideoRepository repository) 
        => _repository = repository;

    public async Task<VideoModelOutput> Handle(
        GetVideoInput input, 
        CancellationToken cancellationToken)
    {
        var video = await _repository.Get(input.VideoId, cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }
}
