using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
public class GenreModelOutput
{
    public GenreModelOutput(
        Guid id, 
        string name, 
        bool isActive, 
        DateTime createdAt,
        IReadOnlyList<GenreModelOutputCategory> categories
    ) {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedAt = createdAt;
        Categories = categories;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<GenreModelOutputCategory> Categories { get; set; }

    public static GenreModelOutput FromGenre(
        DomainEntity.Genre genre
    ) => new(
            genre.Id,
            genre.Name,
            genre.IsActive,
            genre.CreatedAt,
            genre.Categories.Select(
                categoryId => new GenreModelOutputCategory(categoryId)
            ).ToList().AsReadOnly()
        );
}

public class GenreModelOutputCategory
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public GenreModelOutputCategory(Guid id, string? name = null)
        => (Id, Name) = (id, name);
}