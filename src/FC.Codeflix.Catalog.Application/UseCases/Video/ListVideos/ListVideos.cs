using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntities = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository _videoRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;

    public ListVideos(
        IVideoRepository videoRepository,
        ICategoryRepository categoryRepository,
        IGenreRepository genreRepository)
    {
        _videoRepository = videoRepository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
    }

    public async Task<ListVideosOutput> Handle(
        ListVideosInput input, 
        CancellationToken cancellationToken)
    {
        var result = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);
        
        IReadOnlyList<DomainEntities.Category>? categories = null;
        var relatedCategoriesIds = result.Items
            .SelectMany(video => video.Categories).Distinct().ToList();
        if(relatedCategoriesIds is not null && relatedCategoriesIds.Count > 0)
            categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);

        IReadOnlyList<DomainEntities.Genre>? genres = null;
        var relatedGenresIds = result.Items.SelectMany(item => item.Genres).Distinct().ToList();
        if(relatedGenresIds is not null && relatedGenresIds.Count > 0)
            genres = await _genreRepository.GetListByIds(relatedGenresIds, cancellationToken);

        var output = new ListVideosOutput(
            result.CurrentPage, 
            result.PerPage,
            result.Total,
            result.Items.Select(item => VideoModelOutput.FromVideo(item, categories, genres)).ToList());
        return output;
    }
}
