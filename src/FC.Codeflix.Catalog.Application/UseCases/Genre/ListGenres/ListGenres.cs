using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenres
    : IListGenres
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenres(
        IGenreRepository genreRepository, 
        ICategoryRepository categoryRepository
    )
        => (_genreRepository, _categoryRepository) = (genreRepository, categoryRepository);

    public async Task<ListGenresOutput> Handle(
        ListGenresInput input, 
        CancellationToken cancellationToken
    )
    {
        var searchOutput = await _genreRepository.Search(
            input.ToSearchInput(), cancellationToken
        );
        ListGenresOutput output = ListGenresOutput.FromSearchOutput(searchOutput);

        List<Guid> relatedCategoriesIds = searchOutput.Items
            .SelectMany(item => item.Categories).Distinct().ToList();
        if (relatedCategoriesIds.Count > 0)
        {
            IReadOnlyList<DomainEntity.Category> categories =
                await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            output.FillWithCategoryNames(categories);
        }
        return output;
    }
}
