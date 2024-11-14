namespace MetadataHandlerLambda;

public class FileMetadata
{
    public required string File { get; set; }
    
    public required long Timestamp { get; set; }
    
    public required long Size { get; set; }
    
    public string? Version { get; set; }
}