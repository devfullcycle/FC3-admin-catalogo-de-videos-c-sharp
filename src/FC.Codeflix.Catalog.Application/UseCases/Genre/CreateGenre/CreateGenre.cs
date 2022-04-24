using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenre : ICreateGenre
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGenre(
        IGenreRepository genreRepository, 
        IUnitOfWork unitOfWork
    ) {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenreModelOutput> Handle(
        CreateGenreInput request, 
        CancellationToken cancellationToken
    ) {
        var genre = new DomainEntity.Genre(
            request.Name,
            request.IsActive
        );
        if (request.CategoriesIds is not null)
            request.CategoriesIds.ForEach(genre.AddCategory);
        await _genreRepository.Insert(genre, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return GenreModelOutput.FromGenre(genre);
    }
}
