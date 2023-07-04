using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.EventHandlers;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.Producer;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(
        this IServiceCollection services
    )
    {
        services.AddMediatR(typeof(CreateCategory));
        services.AddRepositories();
        services.AddDomainEvents();
        return services;
    }

    private static IServiceCollection AddRepositories(
            this IServiceCollection services
        )
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<ICastMemberRepository, CastMemberRepository>();
        services.AddTransient<IVideoRepository, VideoRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddDomainEvents(
        this IServiceCollection services)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventHandler<VideoUploadedEvent>,
            SendToEncoderEventHandler>();

        return services;
    }


}
