using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
public class CreateGenreInput: 
    IRequest<GenreModelOutput>
{
    public CreateGenreInput(
        string name, 
        bool isActive,
        List<Guid>? categoriesId = null 
    ) {
        Name = name;
        IsActive = isActive;
        CategoriesId = categoriesId;
    }

    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<Guid>? CategoriesId { get; set; }
}
