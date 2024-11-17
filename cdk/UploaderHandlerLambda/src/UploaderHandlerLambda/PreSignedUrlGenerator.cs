using Amazon.S3;
using Amazon.S3.Model;  
using UploaderHandlerLambda.Interfaces;

namespace UploaderHandlerLambda;

public class PreSignedUrlGenerator : IPreSignedUrlGenerator
{

    public Task<string> GeneratePresignedUrl(IAmazonS3 client, string bucketName, string objectKey, double duration)
    {
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddHours(duration),
            };
            return client.GetPreSignedURLAsync(request);
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error:'{ex.Message}'");
            throw;
        }
    }
}