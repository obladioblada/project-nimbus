using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.S3;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, ProjectNimbusStackProps props) : base(scope, id, props)
        {
            //s3 bucket
            var storageBucket = new Bucket(this, $"storage-{props.Config}", new BucketProps {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                Encryption = BucketEncryption.S3_MANAGED,
                EnforceSSL = true,
                Versioned = true,
                RemovalPolicy = RemovalPolicy.RETAIN
            });
            
            // Metadata handler
            var metadataHandlerFunction = new Function(this, $"metadata-handler-{props.Config.env}", new FunctionProps
            {
                Runtime = Runtime.NODEJS_LATEST,
                Code = Code.FromAsset("lib/lambda-handler"),
                Handler = "index.handler"
            });

            metadataHandlerFunction.AddEventSource(new S3EventSourceV2(storageBucket, new S3EventSourceProps
            {
                Events = new[] { EventType.OBJECT_CREATED, EventType.OBJECT_REMOVED }
            }));
            
            //DynamoDb
            var metadataTableAttributes = new
            {
                PartitionKey = new Attribute { Name = "file", Type = AttributeType.STRING },
                SortKey = new Attribute { Name = "Timestamp", Type = AttributeType.NUMBER }
            };
            TablePropsV2 tableProps = new()
            {
                PartitionKey = metadataTableAttributes.PartitionKey,
                SortKey = metadataTableAttributes.SortKey,
                RemovalPolicy = RemovalPolicy.SNAPSHOT,
                TableName = $"metadataTable-{props.Config.env}"
            };
            
            var metadataTable = new TableV2(this, $"metadata-{props.Config.env}", tableProps);
            metadataTable.GrantReadWriteData(metadataHandlerFunction);
        }
    }
}
