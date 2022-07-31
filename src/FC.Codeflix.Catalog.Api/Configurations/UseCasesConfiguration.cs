using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using MediatR;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(
        this IServiceCollection services
    )
    {
        services.AddMediatR(typeof(CreateCategory));
        services.AddRepositories();
        return services;
    }

    private static IServiceCollection AddRepositories(
            this IServiceCollection services
        )
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<ICastMemberRepository, CastMemberRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }


}
