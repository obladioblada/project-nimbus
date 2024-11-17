using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Annotations;
using MetadataHandlerLambda.Interfaces;
using MetadataHandlerLambda.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MetadataHandlerLambda;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.TryAddAWSService<IAmazonDynamoDB>();
        
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>(p =>
            new DynamoDBContext(new AmazonDynamoDBClient(new AmazonDynamoDBConfig()
                {
                    ServiceURL = "http://localhost:4566"
                })
            )
        );

        services.TryAddSingleton<IMetadataService, MetadataService>();
    }
}