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
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideo(
        IVideoRepository videoRepository, 
        ICategoryRepository categoryRepository, 
        IUnitOfWork unitOfWork)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
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

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);
        if (validationHandler.HasErrors())
            throw new EntityValidationException("There are validation errors", 
                validationHandler.Errors);

        if ((input.CategoriesIds?.Count ?? 0) > 0)
        {
            var persistenceIds = await _categoryRepository.GetIdsListByIds(
                input.CategoriesIds!.ToList(), cancellationToken);
            if(persistenceIds.Count < input.CategoriesIds!.Count)
            {
                var notFoundIds = input.CategoriesIds!.ToList()
                    .FindAll(categoryId => !persistenceIds.Contains(categoryId));
                throw new RelatedAggregateException(
                    $"Related category id (or ids) not found: {string.Join(',', notFoundIds)}.");
            }
            input.CategoriesIds!.ToList().ForEach(video.AddCategory);
        }

        await _videoRepository.Insert(video, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return CreateVideoOutput.FromVideo(video);
    }
}
