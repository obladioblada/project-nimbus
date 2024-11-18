using Amazon.DynamoDBv2.DataModel;
using AWS.Lambda.Powertools.Logging;
using MetadataHandlerLambda.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MetadataHandlerLambda.Services;

public class MetadataService : IMetadataService
{
    private readonly IDynamoDBContext _dbContext;
    private readonly DynamoDBOperationConfig _tableOperationConfig;

    public MetadataService(IDynamoDBContext dynamoDbContext, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(dynamoDbContext);
        ArgumentNullException.ThrowIfNull(configuration);
        var tableName = configuration.GetValue<string>("Configuration:MetadataTableName");
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        _dbContext = dynamoDbContext;
        _tableOperationConfig = new DynamoDBOperationConfig
        {
            OverrideTableName = tableName,
        };
    }
    
    public async Task<FileMetadata> SaveAsync(FileMetadata fileMetadata)
    {
        await _dbContext.SaveAsync(fileMetadata, _tableOperationConfig);
        Logger.LogInformation($"Metadata for {fileMetadata.File} stored in dynamoDb.");
        return fileMetadata;
    }
}