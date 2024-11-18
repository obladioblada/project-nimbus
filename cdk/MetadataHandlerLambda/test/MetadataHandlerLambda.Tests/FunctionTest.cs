using Amazon.Lambda.S3Events;
using Amazon.Lambda.TestUtilities;
using MetadataHandlerLambda.Interfaces;
using Moq;
using Xunit;

namespace MetadataHandlerLambda.Tests;

public class FunctionTest
{
    private readonly Mock<IMetadataService> _mockMetadataService = new();
    private const string Bucket = "s3-bucket";
    [Fact]
    public async void TestS3EventLambdaFunctionOk()
    {
        // Arrange
        var fileMetadataExpected = new FileMetadata
        {
            File = "file-test.txt",
            Timestamp = 0000000,
            Size = 10,
            Version = "1"
        };
        
        _mockMetadataService
            .Setup(x => x.SaveAsync(It.IsAny<FileMetadata>()))
            .ReturnsAsync(fileMetadataExpected);

        var s3Event = CreateS3Event(fileMetadataExpected);
        var function = new Function(_mockMetadataService.Object);

        // Act
        var responseList = await function.FunctionHandler(s3Event, new TestLambdaContext());
        
        // Assert
        Assert.Single(responseList);
        Assert.Equal("Principal#s3-bucket#file-test.txt",  string.Join( ",", responseList.ToArray()));
    }
    
    [Fact]
    public async void TestS3EventLambdaFunctionNoRecordObj()
    {
        
        // Arrange
        var s3Event = new S3Event();
        var function = new Function(_mockMetadataService.Object);
        
        // Act
        Func<Task> act = () => function.FunctionHandler(s3Event, new TestLambdaContext());
        
        // Assert
        await Assert.ThrowsAsync<EmptyEventException>(act);
    }
    
    [Fact]
    public async void TestS3EventLambdaFunctionNullS3Event()
    {
        
        // Arrange
        var s3Event = new S3Event {
            Records =
            [
                new S3Event.S3EventNotificationRecord()]
            };
        var function = new Function(_mockMetadataService.Object);
        
        //Act
        Func<Task> act = () => function.FunctionHandler(s3Event, new TestLambdaContext());
        
        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
        Assert.Equal("Value cannot be null. (Parameter 's3Event')", exception.Message);
    }

    private static S3Event CreateS3Event(FileMetadata fileMetadataExpected)
    {
        return new S3Event
        {
            Records =
            [
                new S3Event.S3EventNotificationRecord
                {
                    S3 = new S3Event.S3Entity
                    {
                        Bucket = new S3Event.S3BucketEntity
                        {
                            Name = Bucket,
                            OwnerIdentity = new S3Event.UserIdentityEntity()
                            {
                                PrincipalId = "Principal"
                            }
                        },
                        Object = new S3Event.S3ObjectEntity
                        {
                            Key = fileMetadataExpected.File,
                            Size = fileMetadataExpected.Size,
                            VersionId = fileMetadataExpected.Version
                        }
                    }
                }
            ]
        };
    }
}