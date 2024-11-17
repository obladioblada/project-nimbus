using Amazon.CDK;

namespace Cdk;

public class ProjectNimbusStackProps : StackProps
{
    public required Configuration Config { get; set; }
}