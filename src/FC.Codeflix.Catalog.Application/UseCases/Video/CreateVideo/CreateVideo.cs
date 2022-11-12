using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;

    public CreateVideo(
        IVideoRepository videoRepository,
        ICategoryRepository categoryRepository,
        IGenreRepository genreRepository,
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork,
        IStorageService storageService)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
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

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors",
                validationHandler.Errors);

        await ValidateAndAddRelations(input, video, cancellationToken);

        try
        {
            await UploadImagesMedia(input, video, cancellationToken);
            await UploadVideosMedia(input, video, cancellationToken);

            await _videoRepository.Insert(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return CreateVideoOutput.FromVideo(video);
        }
        catch (Exception)
        {
            await ClearStorage(video, cancellationToken);
            throw;
        }
    }

    private async Task ClearStorage(DomainEntities.Video video, CancellationToken cancellationToken)
    {
        if (video.Thumb is not null)
            await _storageService.Delete(video.Thumb.Path, cancellationToken);
        if (video.ThumbHalf is not null)
            await _storageService.Delete(video.ThumbHalf.Path, cancellationToken);
        if (video.Banner is not null)
            await _storageService.Delete(video.Banner.Path, cancellationToken);
    }

    private async Task UploadImagesMedia(CreateVideoInput input, DomainEntities.Video video, CancellationToken cancellationToken)
    {
        if (input.Thumb is not null)
        {
            var thumbUrl = await _storageService.Upload(
                $"{video.Id}-thumb.{input.Thumb.Extension}",
                input.Thumb.FileStream,
                cancellationToken);
            video.UpdateThumb(thumbUrl);
        }

        if (input.Banner is not null)
        {
            var bannerUrl = await _storageService.Upload(
                $"{video.Id}-banner.{input.Banner.Extension}",
                input.Banner.FileStream,
                cancellationToken);
            video.UpdateBanner(bannerUrl);
        }

        if (input.ThumbHalf is not null)
        {
            var thumbHalfUrl = await _storageService.Upload(
                $"{video.Id}-thumbhalf.{input.ThumbHalf.Extension}",
                input.ThumbHalf.FileStream,
                cancellationToken);
            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private async Task UploadVideosMedia(CreateVideoInput input, DomainEntities.Video video, CancellationToken cancellationToken)
    {
        if (input.Media is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.Media.Extension);
            var mediaUrl = await _storageService.Upload(
                fileName,
                input.Media.FileStream,
                cancellationToken);
            video.UpdateMedia(mediaUrl);
        }
    }

    private async Task ValidateAndAddRelations(CreateVideoInput input, DomainEntities.Video video, CancellationToken cancellationToken)
    {
        if ((input.CategoriesIds?.Count ?? 0) > 0)
        {
            await ValidateCategoriesIds(input, cancellationToken);
            input.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }

        if ((input.GenresIds?.Count ?? 0) > 0)
        {
            await ValidateGenresIds(input, cancellationToken);
            input.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        if ((input.CastMembersIds?.Count ?? 0) > 0)
        {
            await ValidateCastMembersIds(input, cancellationToken);
            input.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateCastMembersIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _castMemberRepository.GetIdsListByIds(
            input.CastMembersIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CastMembersIds!.Count)
        {
            var notFoundIds = input.CastMembersIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related cast member id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateGenresIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _genreRepository.GetIdsListByIds(
            input.GenresIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.GenresIds!.Count)
        {
            var notFoundIds = input.GenresIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateCategoriesIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _categoryRepository.GetIdsListByIds(
            input.CategoriesIds!.ToList(), cancellationToken);
        if (persistenceIds.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds!.ToList()
                .FindAll(categoryId => !persistenceIds.Contains(categoryId));
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }
}
