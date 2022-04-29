using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
public class ListGenres
    : IListGenres
{
    private readonly IGenreRepository _genreRepository;

    public ListGenres(IGenreRepository genreRepository) 
        => _genreRepository = genreRepository;

    public async Task<ListGenresOutput> Handle(
        ListGenresInput input, 
        CancellationToken cancellationToken
    )
    {
        var searchInput = new SearchInput(
            input.Page,
            input.PerPage,
            input.Search,
            input.Sort,
            input.Dir
        );
        var searchOutput = await _genreRepository.Search(
            searchInput, 
            cancellationToken
        );
        return new ListGenresOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(GenreModelOutput.FromGenre)
                .ToList()
        );
    }
}
