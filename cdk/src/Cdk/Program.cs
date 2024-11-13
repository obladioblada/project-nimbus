using System;
using Amazon.CDK;
using Microsoft.Extensions.Configuration;

namespace Cdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var configuration = GetConfiguration();
            var stack = new CdkStack(app, "ProjectNimbusStack", new ProjectNimbusStackProps
            {
               Config = configuration
            });
            
            app.Synth();
        }
        
        private static Configuration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("configuration.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            return config.Get<Configuration>() ?? throw new Exception("Missing configuration");
        }
    }
    
    
}
