using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
public interface IDeleteCategory 
    : IRequestHandler<DeleteCategoryInput>
{}
