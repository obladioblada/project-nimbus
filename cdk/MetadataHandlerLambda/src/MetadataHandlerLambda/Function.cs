using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Runtime.Internal.Util;
using MetadataHandlerLambda.Interfaces;
using Microsoft.Extensions.Logging;
using Logger = AWS.Lambda.Powertools.Logging.Logger;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MetadataHandlerLambda;

public class Function
{
    private readonly IMetadataService _metadataService;
    public Function(IMetadataService metadataService)
    {
        ArgumentNullException.ThrowIfNull(metadataService);
        _metadataService = metadataService;
    }
    /// <summary>This method takes in an S3 event object and store the metadata to dynamoDb table
    /// </summary>
    /// <param name="evnt">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    [LambdaFunction]
    public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        Logger.LogInformation("Got event from s3");
        var records = evnt.Records ?? [];
        if (records.Count == 0)
        {
            Logger.LogWarning("No records found.");
            return "No records found";
        }
        
        foreach (var record in evnt.Records ?? [])
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                Logger.LogWarning("Empty s3 event");
                continue;
            }
            
            try
            {
                var metadata = new FileMetadata
                {
                    File = $"{s3Event.Bucket.OwnerIdentity.PrincipalId}#{s3Event.Bucket.Name}#{s3Event.Object.Key}",
                    Timestamp = record.EventTime.ToUniversalTime().Ticks,
                    Size = s3Event.Object.Size,
                    Version = s3Event.Object.VersionId
                };

                await _metadataService.Save(metadata);
                Logger.LogInformation($"Metadata for {metadata.File} stored in dynamoDb.");
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw;
            }
        }
        return "ok";
    }
}