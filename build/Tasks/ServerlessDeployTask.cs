using Cake.Common;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Frosting;
using Build.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Cake.CloudFormation;

namespace Build.Tasks
{
    [TaskName(Constants.TaskServerlessDeploy)]
    public class ServerlessDeployTask : FrostingTask<BuildContext>
    {


        public override void Run(BuildContext context)
        {
            if (!context.TeamCity().IsRunningOnTeamCity)
            {
                var applicationEnvironment = Environment.GetEnvironmentVariable("Application__Environment");

                var template = context.File("./template.yaml");

                // TODO: Get it from metadata
                var applicationSystem = context.Options.ApplicationSystem;
                var applicationSubsystem = context.Options.ApplicationSubsystem;
                var applicationPlatform = context.Options.ApplicationPlatfrom;
                var applicationOwner = context.Options.ApplicationOwner;

                var version = Environment.GetEnvironmentVariable("Application__Version") ?? context.Options.ApplicationVersion; 
                Environment.SetEnvironmentVariable("Application__Version", version);
                
                var s3Prefix = string.Join("/", new List<string> { applicationPlatform, applicationSystem, applicationSubsystem, version });
                var stackName = string.Join("-", new List<string> { applicationEnvironment, applicationPlatform, applicationSystem, applicationSubsystem });
                string profile = Environment.GetEnvironmentVariable("AWS_PROFILE") ?? Environment.GetEnvironmentVariable("AWS_PROFILE");


                context.CloudFormationDeploy(new DeployArguments
                {
                    Profile = profile,
                    StackName = stackName.Replace("_", string.Empty),
                    //S3Bucket = Environment.GetEnvironmentVariable("Application__ArtifactsS3Bucket"),
                    //S3Prefix = s3Prefix,
                    TemplateFile = template,
                    Capabilities = new List<string> { "CAPABILITY_IAM", "CAPABILITY_NAMED_IAM", "CAPABILITY_AUTO_EXPAND" },
                    ParameterOverrides = GetParametersOverrides(context),
                    RoleArn = Environment.GetEnvironmentVariable("Application__RoleArn") ?? "arn:aws:iam::637422166946:role/sam-cloudformation-role",
                    Tags = GetTags(applicationEnvironment, applicationSystem, applicationSubsystem, applicationPlatform, applicationOwner)
                });
            }
        }
        private Dictionary<string, string> GetParametersOverrides(BuildContext context, string prefix = "Application__")
        {
            return context
                .EnvironmentVariables()
                .Where(kv=>kv.Key.StartsWith(prefix) || kv.Key.StartsWith("AWS"))
                .ToDictionary(x =>
                x.Key.Replace(prefix, string.Empty),
                x => x.Value);
        }

        private static Dictionary<string, string> GetTags(string applicationEnvironment, string applicationSystem, string applicationSubsystem, string applicationPlatform, string applicationOwner)
        {
            return new Dictionary<string, string> {
                        { "Environment",applicationEnvironment },
                        { "Platform",applicationPlatform },
                        { "applicationSystem",applicationSystem },
                        { "Subsystem",applicationSubsystem },
                        { "Provisioner",Environment.GetEnvironmentVariable("Application__Provisioner") },
                        { "Owner",applicationOwner }
                    };
        }
    }
}
