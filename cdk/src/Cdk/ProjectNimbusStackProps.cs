using Amazon.CDK;

namespace Cdk;

public class ProjectNimbusStackProps : StackProps
{
    public Configuration Config { get; set; }
}