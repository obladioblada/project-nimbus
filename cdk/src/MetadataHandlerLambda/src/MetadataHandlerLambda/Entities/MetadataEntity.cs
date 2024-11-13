using Amazon.DynamoDBv2.DataModel;

namespace MetadataHandlerLambda.Entities;

public class MetadataEntity
{
    [DynamoDBHashKey]
    public required string GameId { get; set; }
    
    [DynamoDBRangeKey]
    public required string Timestamps { get; set; }
    
    [DynamoDBProperty]
    public required string S3Uri { get; set; }
}