using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using FC.Codeflix.Catalog.Infra.Storage.Services;
using Google.Cloud.Storage.V1;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(_ => StorageClient.Create());
        services.Configure<StorageServiceOptions>(
            configuration.GetSection(StorageServiceOptions.ConfigurationSection));
        services.AddTransient<IStorageService, StorageService>();
        return services;
    }
}
