using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
public interface ICreateCategory : 
    IRequestHandler<CreateCategoryInput, CategoryModelOutput>
{ }
