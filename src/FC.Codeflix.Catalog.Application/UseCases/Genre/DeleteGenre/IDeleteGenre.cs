using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
public interface IDeleteGenre
    : IRequestHandler<DeleteGenreInput>
{ }
