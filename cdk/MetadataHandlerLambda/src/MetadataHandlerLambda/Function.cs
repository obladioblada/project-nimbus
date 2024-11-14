using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using MetadataHandlerLambda.Interfaces;

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
    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var eventRecords = evnt.Records ?? [];
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)  continue;
            
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
            }
            catch (Exception e)
            {
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }
    }
}