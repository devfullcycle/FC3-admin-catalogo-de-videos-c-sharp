using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using System.Net.Http.Headers;
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

        if ((input.CastMembersIds?.Count ?? 0 ) > 0)
        {
            await ValidateCastMembersIds(input, cancellationToken);
            input.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }

        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CreateVideoOutput.FromVideo(video);
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
