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
        try
        {
            await UploadVideo(input, video, cancellationToken);
            await UploadTrailer(input, video, cancellationToken);
            await UploadImages(input, video, cancellationToken);

            await _videoRepository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return Unit.Value;
        }
        catch (Exception)
        {
            await ClearStorage(input, video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.VideoFile is not null && video.Media is not null)
            await _storageService.Delete(video.Media.FilePath, cancellationToken);
        if (input.TrailerFile is not null && video.Trailer is not null)
            await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
        if (input.BannerFile is not null && video.Banner is not null)
            await _storageService.Delete(video.Banner.Path, cancellationToken);
        if (input.ThumbFile is not null && video.Thumb is not null)
            await _storageService.Delete(video.Thumb.Path, cancellationToken);
        if (input.ThumbHalfFile is not null && video.ThumbHalf is not null)
            await _storageService.Delete(video.ThumbHalf.Path, cancellationToken);
    }

    private async Task UploadImages(UploadMediasInput input,
        Domain.Entity.Video video,
        CancellationToken cancellationToken)
    {
        if (input.BannerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.BannerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                input.BannerFile.FileStream,
                input.BannerFile.ContentType,
                cancellationToken);
            video.UpdateBanner(uploadedFilePath);
        }

        if (input.ThumbFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.ThumbFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                input.ThumbFile.FileStream,
                input.ThumbFile.ContentType,
                cancellationToken);
            video.UpdateThumb(uploadedFilePath);
        }

        if (input.ThumbHalfFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalfFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                input.ThumbHalfFile.FileStream,
                input.ThumbHalfFile.ContentType,
                cancellationToken);
            video.UpdateThumbHalf(uploadedFilePath);
        }
    }

    private async Task UploadTrailer(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
    {
        if (input.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.TrailerFile.Extension);
            var uploadedFilePath = await _storageService.Upload(
                fileName,
                input.TrailerFile.FileStream,
                input.TrailerFile.ContentType,
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
                input.VideoFile.ContentType,
                cancellationToken);
            video.UpdateMedia(uploadedFilePath);
        }
    }
}
