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
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>, IDisposable
    where TStartup : class
{
    private const string VideoCreatedRoutingKey = "video.created";
    public string VideoEncodedRoutingKey => "video.encoded";
    public string VideoCreatedQueue => "video.created.queue";
    public Mock<StorageClient> StorageClient { get; private set; }
    public IModel RabbitMQChannel { get; private set; }
    public RabbitMQConfiguration RabbitMQConfiguration { get; private set; }
    protected override void ConfigureWebHost(
        IWebHostBuilder builder
    )
    {
        var environment = "EndToEndTest";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        builder.UseEnvironment(environment);
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
            SetupRabbitMQ();
            var context = scope.ServiceProvider
                .GetService<CodeflixCatalogDbContext>();
            ArgumentNullException.ThrowIfNull(context);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }

    public void SetupRabbitMQ()
    {
        var channel = RabbitMQChannel!;
        var exchange = RabbitMQConfiguration!.Exchange;
        channel.ExchangeDeclare(exchange, "direct", true, false, null);
        channel.QueueDeclare(VideoCreatedQueue, true, false, false, null);
        channel.QueueBind(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        channel.QueueDeclare(RabbitMQConfiguration.VideoEncodedQueue,
            true, false, false, null);
        channel.QueueBind(RabbitMQConfiguration.VideoEncodedQueue,
            exchange, VideoEncodedRoutingKey, null);
    }

    private void TearDownRabbitMQ()
    {
        var channel = RabbitMQChannel!;
        var exchange = RabbitMQConfiguration!.Exchange;
        channel.QueueUnbind(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        channel.QueueDelete(VideoCreatedQueue, false, false);
        channel.QueueUnbind(RabbitMQConfiguration.VideoEncodedQueue,
            exchange, VideoEncodedRoutingKey, null);
        channel.QueueDelete(RabbitMQConfiguration.VideoEncodedQueue, false, false);
        channel.ExchangeDelete(exchange, false);
    }

    public override ValueTask DisposeAsync()
    {
        TearDownRabbitMQ();
        return base.DisposeAsync();
    }
}

