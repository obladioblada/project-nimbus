using Amazon.DynamoDBv2.DataModel;
using MetadataHandlerLambda.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MetadataHandlerLambda;

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
            OverrideTableName = tableName
        };
    }
    
    public Task Save(CancellationToken cancellationToken)
    {
        return _dbContext.SaveAsync("",_tableOperationConfig, cancellationToken);
    }

    public Task Delete(CancellationToken cancellationToken)
    {
        return _dbContext.DeleteAsync("", _tableOperationConfig, cancellationToken);
    }
}