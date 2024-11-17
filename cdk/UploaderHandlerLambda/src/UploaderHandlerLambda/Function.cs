using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using UploaderHandlerLambda.Interfaces;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UploaderHandlerLambda;

public class Function
{
    private readonly IPreSignedUrlGenerator _preSignedUrlGenerator;
    private readonly IAmazonS3 _amazonS3;
    private readonly string _bucketName;
    
    public Function(IConfiguration configuration, IAmazonS3 amazonS3, IPreSignedUrlGenerator preSignedUrlGenerator)
    {
        ArgumentNullException.ThrowIfNull(preSignedUrlGenerator);
        ArgumentNullException.ThrowIfNull(amazonS3);
        ArgumentNullException.ThrowIfNull(configuration);
        var bucketName = configuration.GetValue<string>("S3Configuration:StorageBucketName");
        ArgumentNullException.ThrowIfNull(bucketName);
        _bucketName = bucketName;
        _preSignedUrlGenerator = preSignedUrlGenerator;
        _amazonS3 = amazonS3;

    }
    /// <summary>This method takes a file name string and generate a presigned URL to be used to upload a file to s3
    /// </summary>
    /// <param name="input">The function input cref="Input"</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    [LambdaFunction]
    public async Task<string> FunctionHandler(Input input, ILambdaContext context)
    {
        return await _preSignedUrlGenerator.GeneratePresignedUrl(_amazonS3, _bucketName, input.FileName, 12);
    }
}