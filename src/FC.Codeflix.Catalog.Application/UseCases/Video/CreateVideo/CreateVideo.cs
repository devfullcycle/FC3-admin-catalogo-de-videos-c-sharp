using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateVideoOutput> Handle(
        CreateVideoInput input, 
        CancellationToken cancellationToken)
    {
        var video = new DomainEntities.Video(
            input.Title,
            input.Description,
            input.YearLaunched,
            input.Opened,
            input.Published,
            input.Duration,
            input.Rating);

        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return new CreateVideoOutput(
            video.Id,
            video.CreatedAt,
            video.Title,
            video.Published,
            video.Description,
            video.Rating,
            video.YearLaunched,
            video.Opened,
            video.Duration);
    }
}
