using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
public interface IListCategories
    : IRequestHandler<ListCategoriesInput, ListCategoriesOutput>
{}
