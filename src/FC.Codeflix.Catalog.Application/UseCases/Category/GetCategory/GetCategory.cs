using FC.Codeflix.Catalog.Domain.Repository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
public class GetCategory : IRequestHandler<GetCategoryInput, GetCategoryOutput>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategory(ICategoryRepository categoryRepository) 
        => _categoryRepository = categoryRepository;

    public async Task<GetCategoryOutput> Handle(
        GetCategoryInput request, 
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryRepository.Get(request.Id, cancellationToken);
        return GetCategoryOutput.FromCategory(category);
    }
}
