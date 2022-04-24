using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenreInput: 
    IRequest<GenreModelOutput>
{
    public CreateGenreInput(
        string name, 
        bool isActive
    ) {
        Name = name;
        IsActive = isActive;
    }

    public string Name { get; set; }
    public bool IsActive { get; set; }
}
