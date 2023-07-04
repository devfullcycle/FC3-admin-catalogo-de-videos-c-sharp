using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.DTOs;
using FC.Codeflix.Catalog.Infra.Messaging.JsonPolicies;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Infra.Messaging.Consumer;
public class VideoEncodedEventConsumer : BackgroundService
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
        string messageString = string.Empty;
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var rawMessage = eventArgs.Body.ToArray();
            messageString = Encoding.UTF8.GetString(rawMessage);
            _logger.LogDebug(messageString);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCasePolicy()
            };
            var message = JsonSerializer
                .Deserialize<VideoEncodedMessageDTO>(messageString, jsonOptions);
            var input = GetUpdateMediaStatusInput(message!);
            mediator.Send(input, CancellationToken.None).Wait();
            _channel.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
            when (ex is EntityValidationException or NotFoundException)
        {
            _logger.LogError(ex,
                "There was a business error in the message processing: {delivertTag}, {message}",
                eventArgs.DeliveryTag, messageString);
            _channel.BasicNack(eventArgs.DeliveryTag, false, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "There was a unexpected error in the message processing: {delivertTag}, {message}",
                eventArgs.DeliveryTag, messageString);
            _channel.BasicNack(eventArgs.DeliveryTag, false, true);
        }

    }

    private UpdateMediaStatusInput GetUpdateMediaStatusInput(
        VideoEncodedMessageDTO message)
    {
        if (message!.Video != null)
        {
            return new UpdateMediaStatusInput(
                Guid.Parse(message.Video!.ResourceId!),
                MediaStatus.Completed,
                EncodedPath: message.Video.FullEncodedVideoFilePath);
        }

        return new UpdateMediaStatusInput(
            Guid.Parse(message.Message!.ResourceId!),
            MediaStatus.Error,
            ErrorMessage: message.Error);
    }
}
