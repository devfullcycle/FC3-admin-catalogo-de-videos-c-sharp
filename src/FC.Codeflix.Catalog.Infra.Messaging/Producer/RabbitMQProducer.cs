using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.JsonPolicies;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Infra.Messaging.Producer;
public class RabbitMQProducer : IMessageProducer
{
    private readonly IModel _channel;
    private readonly string _exchange;

    public RabbitMQProducer(IModel channel,
        IOptions<RabbitMQConfiguration> options)
    {
        _channel = channel;
        _exchange = options.Value.Exchange!;
    }

    public Task SendMessageAsync<T>(T message, CancellationToken cancellationToken)
    {
        var routingKey = EventsMapping.GetRoutingKey<T>();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy()
        };
        var @event = JsonSerializer.SerializeToUtf8Bytes(
            message, jsonOptions);
        _channel.BasicPublish(
            exchange: _exchange,
            routingKey: routingKey,
            body: @event);
        _channel.WaitForConfirmsOrDie();
        return Task.CompletedTask;
    }
}
