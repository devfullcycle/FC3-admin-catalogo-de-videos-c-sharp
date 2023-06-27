using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
public class UpdateMediaStatus : IUpdateMediaStatus
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMediaStatus> _logger;

    public UpdateMediaStatus(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMediaStatus> logger)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<VideoModelOutput> Handle(
        UpdateMediaStatusInput request,
        CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(request.VideoId, cancellationToken);

        switch (request.Status)
        {
            case MediaStatus.Completed:
                video.UpdateAsEncoded(request.EncodedPath!);
                break;
            case MediaStatus.Error:
                _logger.LogError(
                    "There was an error while trying to encode the video {videoId}: {error}",
                    video.Id, request.ErrorMessage);
                video.UpdateAsEncodingError();
                break;
            default:
                throw new EntityValidationException("Invalid media status");
        }

        await _videoRepository.Update(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }
}
