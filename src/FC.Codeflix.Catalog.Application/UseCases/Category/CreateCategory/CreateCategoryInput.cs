namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
public class CreateCategoryInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }

    public CreateCategoryInput(
        string name,
        string? description = null,
        bool isActive = true)
    {
        Name = name;
        Description = description ?? "";
        IsActive = isActive;
    }
}
