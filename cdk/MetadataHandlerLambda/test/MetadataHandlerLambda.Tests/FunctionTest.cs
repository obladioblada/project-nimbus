using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.TestUtilities;
using Amazon.S3;
using Amazon.S3.Model;
using MetadataHandlerLambda.Interfaces;
using Moq;
using Xunit;

namespace MetadataHandlerLambda.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestS3EventLambdaFunction()
    {
        var mockMetadataService = new Mock<IMetadataService>();
        var getObjectMetadataResponse = new GetObjectMetadataResponse
        {
            Headers =
            {
                ContentType = "text/plain"
            }
        };

        mockMetadataService
            .Setup(x => x.Save(It.IsAny<FileMetadata>()))
            .Returns(Task.FromResult(getObjectMetadataResponse));

        // Setup the S3 event object that S3 notifications would create with the fields used by the Lambda function.
        var s3Event = new S3Event
        {
            Records = new List<S3Event.S3EventNotificationRecord>
            {
                new S3Event.S3EventNotificationRecord
                {
                    S3 = new S3Event.S3Entity
                    {
                        Bucket = new S3Event.S3BucketEntity {Name = "s3-bucket" },
                        Object = new S3Event.S3ObjectEntity {Key = "text.txt" }
                    }
                }
            }
        };

        // Invoke the lambda function and confirm the content type was returned.
        ILambdaLogger testLambdaLogger = new TestLambdaLogger();
        var testLambdaContext = new TestLambdaContext
        {
            Logger = testLambdaLogger
        };

        var function = new Function(mockMetadataService.Object);
        await function.FunctionHandler(s3Event, testLambdaContext);

        Assert.Equal("text/plain", ((TestLambdaLogger)testLambdaLogger).Buffer.ToString().Trim());
    }
}