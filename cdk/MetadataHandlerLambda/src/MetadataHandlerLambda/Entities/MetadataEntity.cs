using Amazon.DynamoDBv2.DataModel;

namespace MetadataHandlerLambda.Entities;

public class MetadataEntity
{
    [DynamoDBHashKey]
    public required string File { get; set; }
    
    [DynamoDBRangeKey]
    public required DateTime Timestamps { get; set; }
    
    [DynamoDBProperty]
    public required string S3Uri { get; set; }
    
    [DynamoDBProperty]
    public required long Size { get; set; }
    
    [DynamoDBProperty]
    public long Version { get; set; }
}