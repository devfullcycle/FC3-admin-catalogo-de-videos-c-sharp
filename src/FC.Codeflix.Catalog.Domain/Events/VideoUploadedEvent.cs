using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Events;
public class VideoUploadedEvent : DomainEvent
{
    public VideoUploadedEvent(Guid resourceId, string filePath)
        : base()
    {
        ResourceId = resourceId;
        FilePath = filePath;
    }

    public Guid ResourceId { get; set; }
    public string FilePath { get; set; }
}
