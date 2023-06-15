using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public Mock<StorageClient> StorageClient { get; private set; }
    public IModel RabbitMQChannel { get; private set; }
    public RabbitMQConfiguration RabbitMQConfiguration { get; private set; }
    protected override void ConfigureWebHost(
        IWebHostBuilder builder
    )
    {
        builder.UseEnvironment("EndToEndTest");
        builder.ConfigureServices(services => {
            var descriptor = services.First(s =>
                s.ServiceType == typeof(StorageClient));
            services.Remove(descriptor);
            services.AddScoped(sp =>
            {
                StorageClient = new Mock<StorageClient>();
                return StorageClient.Object;
            });
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            RabbitMQChannel = scope
                .ServiceProvider
                .GetService<ChannelManager>()!
                .GetChannel();
            RabbitMQConfiguration = scope
                .ServiceProvider
                .GetService<IOptions<RabbitMQConfiguration>>()!
                .Value;
            var context = scope.ServiceProvider
                .GetService<CodeflixCatalogDbContext>();
            ArgumentNullException.ThrowIfNull(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }
}
