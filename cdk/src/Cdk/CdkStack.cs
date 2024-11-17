using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.S3.Notifications;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, ProjectNimbusStackProps props) : base(scope, id, props)
        {
            //s3 bucket CORS Rule (Allow all for testing)
            var corsRule = new CorsRule
            {
                AllowedMethods = [HttpMethods.GET, HttpMethods.PUT, HttpMethods.POST],
                AllowedOrigins = ["*"],
                AllowedHeaders = ["*"],
                Id = $"storage-bucket-CORS-rule-{props.Config.env}",
                MaxAge = 3600
            };
            var storageBucket = CreateS3Bucket(props.Config.env, corsRule);

            //DynamoDb Table
            var metadataTable = CreateDynamoDbTable(
                "metadata-table",  
                new Attribute { Name = "File", Type = AttributeType.STRING },
                new Attribute { Name = "Timestamp", Type = AttributeType.NUMBER },
                props.Config.env
                );

            // Metadata handler
            var metadataHandlerFunction = CreateLambda(
                "metadata-handler",
                props.Config.env,
                "MetadataHandlerLambda::MetadataHandlerLambda.Function::FunctionHandler",
                "lib/metadata-lambda-handler.zip",
                new Dictionary<string, string>
                {
                    { "Configuration__MetadataTableName", metadataTable.TableName }
                });

            //this seems not working in localstack
            // https://stackoverflow.com/questions/78311472/aws-cdk-create-s3-event-notification-to-sqs-message-in-localstack
            storageBucket.AddEventNotification(
                EventType.OBJECT_CREATED_COMPLETE_MULTIPART_UPLOAD,
                new LambdaDestination(metadataHandlerFunction));
            
            metadataTable.GrantReadWriteData(metadataHandlerFunction);
            
            // uploader handler
            var uploaderHandlerFunction = CreateLambda(
                "uploader-handler",
                props.Config.env,
                "UploaderHandlerLambda::UploaderHandlerLambda.Function_FunctionHandler_Generated::FunctionHandler",
                "lib/uploader-lambda-handler.zip",
                new Dictionary<string, string>
                {
                    { "S3Configuration__StorageBucketName", storageBucket.BucketName }
                });

            CreateCfnOutput($"metadata-handler-lambda-{props.Config.env}-arn", metadataHandlerFunction.FunctionArn);
            CreateCfnOutput($"metadata-table-name-{props.Config.env}", metadataTable.TableName);
            CreateCfnOutput($"s3-bucket-storage-name-{props.Config.env}", storageBucket.BucketName);
        }

        private Function CreateLambda(
            string id,
            string env,
            string handler,
            string codePath,
            IDictionary<string, string>? environmentVariables = null
        )
        {
            return new Function(this, id, new FunctionProps
            {
                FunctionName = $"{id}-{env}",
                Runtime = Runtime.DOTNET_8,
                Handler = handler,
                Code = Code.FromAsset(codePath),
                Environment = environmentVariables
            });
        }
        
        private ITable CreateDynamoDbTable(string name, IAttribute pk, IAttribute sk, string env)
        {
            TablePropsV2 tableProps = new()
            {
                PartitionKey = pk,
                SortKey = sk,
                RemovalPolicy = RemovalPolicy.RETAIN,
                TableName = $"{name}-{env}"
            };

            return new TableV2(this, tableProps.TableName, tableProps);
        }

        private Bucket CreateS3Bucket(string env, CorsRule corsRule)
        {
            //s3 bucket
            var storageBucket = new Bucket(this, $"storage-{env}", new BucketProps
            {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                Encryption = BucketEncryption.S3_MANAGED,
                EnforceSSL = true,
                Versioned = true,
                RemovalPolicy = RemovalPolicy.RETAIN,
                Cors = [corsRule],
                BucketName = $"storage-{env}"
            });
            return storageBucket;
        }


        private void CreateCfnOutput(string key, string value)
        {
            new CfnOutput(this, key, new CfnOutputProps
            {
                Value = value,
                ExportName = key
            });
        }
    }
}