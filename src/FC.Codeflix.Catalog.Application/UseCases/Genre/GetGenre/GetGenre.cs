using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
public class GetGenre
    : IGetGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetGenre(
        IGenreRepository genreRepository,
        ICategoryRepository categoryRepository)
    {
        _genreRepository = genreRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<GenreModelOutput> Handle(
        GetGenreInput request, 
        CancellationToken cancellationToken
    )
    {
        var genre = await _genreRepository
            .Get(request.Id, cancellationToken);
        var output = GenreModelOutput.FromGenre(genre);


        return output;
    }
}
