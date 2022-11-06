using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;

public class DeleteVideo : IDeleteVideo
{
    private readonly IVideoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public DeleteVideo(
        IVideoRepository repository, 
        IUnitOfWork unitOfWork, 
        IStorageService storageService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
    }

    public async Task<Unit> Handle(
        DeleteVideoInput input, 
        CancellationToken cancellationToken)
    {
        var video = await _repository.Get(input.VideoId, cancellationToken);
        
        await _repository.Delete(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        if (video.Trailer is not null)
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);

        if (video.Media is not null)
            await _storageService.Delete(video.Media.FilePath, cancellationToken);

        return Unit.Value;
    }
}
