using Amazon.Lambda.Annotations;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UploaderHandlerLambda.Interfaces;

namespace UploaderHandlerLambda;

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
        services.TryAddSingleton<IPreSignedUrlGenerator, PreSignedUrlGenerator>();
        
        services.TryAddAWSService<IAmazonS3>();
        services.AddSingleton<IAmazonS3, AmazonS3Client>(p => new AmazonS3Client(new AmazonS3Config
        { 
            ServiceURL = "http://localhost:4566",
            ForcePathStyle = true
        }));
    }
}