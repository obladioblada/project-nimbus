using Amazon.CDK;
using Constructs;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            new Bucket(scope, "Bucket", new BucketProps {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                Encryption = BucketEncryption.S3_MANAGED,
                EnforceSSL = true,
                Versioned = true,
                RemovalPolicy = RemovalPolicy.RETAIN
            });

        }
    }
}
