using Build.Extensions;
using Cake.CloudFormation;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using System.Collections.Generic;

namespace Build.Tasks
{

    [TaskName("deploy-ecr")]
    public class EcrDeployBuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.IsReleasableBranch())
            {

                if (context.TeamCity().IsRunningOnTeamCity)
                {
                    System.Environment.SetEnvironmentVariable("AWS_PROFILE", null);
                    context.Information("Set AWS_PROFILE to null");
                }

                context.Log.Information("deploying ECR..");
                string stackName = @$"{context.Options.ApplicationPlatfrom}-{context.Options.ApplicationSystem}-{context.Options.ApplicationSubsystem}-{context.Options.ApplicationName}-Ecr-stack"
                        .Replace("_", "-")
                        .ToLower();
                context.CloudFormationDeploy(new DeployArguments
                {
                    StackName = stackName,
                    TemplateFile = "template-ecr.yaml",
                    ParameterOverrides = new Dictionary<string, string>
            {{"Environment", context.Options.ApplicationEnvironment}},
                    Capabilities = new List<string> { "CAPABILITY_IAM", "CAPABILITY_NAMED_IAM", "CAPABILITY_AUTO_EXPAND" }
                });
                context.Information("Successfully deploy ECR stack ");
            }
        }
    }
}
