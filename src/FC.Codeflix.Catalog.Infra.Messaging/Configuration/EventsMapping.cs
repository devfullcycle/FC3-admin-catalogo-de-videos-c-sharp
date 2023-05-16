using FC.Codeflix.Catalog.Domain.Events;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;
internal static class EventsMapping
{
    private static Dictionary<string, string> _routingKeys => new()
    {
        { typeof(VideoUploadedEvent).Name, "video.created" }
    };

    public static string GetRoutingKey<T>() => _routingKeys[typeof(T).Name];
}
