using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;

public class UploadMedias : IUploadMedias
{
    private readonly IVideoRepository _videoRepository;
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadMedias(
        IVideoRepository videoRepository, 
        IStorageService storageService, 
        IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        UploadMediasInput input, 
        CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(input.VideoId, cancellationToken);
        
        await UploadVideo(input, video, cancellationToken);
        await UploadTrailer(input, video, cancellationToken);

        await _videoRepository.Update(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }

    private async Task UploadTrailer(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.TrailerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                input.TrailerFile.FileStream,
                cancellationToken);
            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private async Task UploadVideo(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.VideoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                input.VideoFile.FileStream,
                cancellationToken);
            video.UpdateMedia(uploadedFilePath);
        }
    }
}
