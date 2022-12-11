using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;

public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository _videoRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICastMemberRepository _castMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVideo(
        IVideoRepository videoRepository,
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository,
        ICastMemberRepository castMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
        _castMemberRepository = castMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<VideoModelOutput> Handle(
        UpdateVideoInput input, 
        CancellationToken cancellationToken)
    {
        var video = await _videoRepository.Get(input.VideoId, cancellationToken);
        video.Update(
            input.Title,
            input.Description,
            input.YearLaunched,
            input.Opened,
            input.Published,
            input.Duration,
            input.Rating);

        await ValidateAndAddRelations(input, video, cancellationToken);

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if(validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors",
                validationHandler.Errors);

        await _videoRepository.Update(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }

    private async Task ValidateAndAddRelations(
        UpdateVideoInput input, 
        DomainEntities.Video video, 
        CancellationToken cancellationToken)
    {
        if((input.GenresIds?.Count ?? 0) > 0)
        {
            await ValidateGenresIds(input, cancellationToken);
            video.RemoveAllGenres();
            input.GenresIds!.ToList().ForEach(video.AddGenre);
        }

        if((input.CategoriesIds?.Count ?? 0) > 0)
        {
            await ValidateCategoriesIds(input, cancellationToken);
            video.RemoveAllCategories();
            input.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }

        if((input.CastMembersIds?.Count ?? 0) > 0)
        {
            await ValidateCastMembersIds(input, cancellationToken);
            video.RemoveAllCastMembers();
            input.CastMembersIds!.ToList().ForEach(video.AddCastMember);
        }
    }

    private async Task ValidateGenresIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _genreRepository.GetIdsListByIds(
            input.GenresIds!.ToList(), cancellationToken);
        if(persistenceIds.Count < input.GenresIds!.Count)
        {
            var notFoundIds = input.GenresIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related genre id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateCategoriesIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _categoryRepository.GetIdsListByIds(
            input.CategoriesIds!.ToList(), cancellationToken);
        if(persistenceIds.Count < input.CategoriesIds!.Count)
        {
            var notFoundIds = input.CategoriesIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }

    private async Task ValidateCastMembersIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds = await _castMemberRepository.GetIdsListByIds(
            input.CastMembersIds!.ToList(), cancellationToken);
        if(persistenceIds.Count < input.CastMembersIds!.Count)
        {
            var notFoundIds = input.CastMembersIds!.ToList()
                .FindAll(id => !persistenceIds.Contains(id));
            throw new RelatedAggregateException(
                $"Related cast member(s) id (or ids) not found: {string.Join(',', notFoundIds)}.");
        }
    }
}
