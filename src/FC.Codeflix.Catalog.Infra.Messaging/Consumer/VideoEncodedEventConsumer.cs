using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Infra.Messaging.Consumer;
internal class VideoEncodedEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VideoEncodedEventConsumer> _logger;
    private readonly string _queue;
    private readonly IModel _channel;

    public VideoEncodedEventConsumer(
        IServiceProvider serviceProvider,
        ILogger<VideoEncodedEventConsumer> logger,
        IOptions<RabbitMQConfiguration> configuration,
        IModel channel)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _queue = configuration.Value!.VideoEncodedQueue!;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnMessageReceived;
        _channel.BasicConsume(
            queue: _queue,
            autoAck: false,
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100000, stoppingToken);
        }
        _logger.LogWarning("Disposing chanel.");
        _channel.Dispose();
    }

    private void OnMessageReceived(
        object? sender,
        BasicDeliverEventArgs eventArgs)
    {

    }
}
