namespace FC.Codeflix.Catalog.Infra.Messaging.DTOs;
public class VideoEncodedMessageDTO
{
    public VideoEncodedMetadataDTO? Video { get; set; }
    public VideoEncodedMetadataDTO? Message { get; set; }
    public string? Error { get; set; }
}

public class VideoEncodedMetadataDTO
{
    public string? EncodedVideoFolder { get; set; }
    public string? FilePath { get; set; }
    public string? ResourceId { get; set; }
}
