using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public interface ICreateGenre
    : IRequestHandler<CreateGenreInput, GenreModelOutput>
{
}
