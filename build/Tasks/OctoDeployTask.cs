using Cake.Frosting;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.OctopusDeploy;
using Build.Extensions;
using System.Collections.Generic;
using System;
using Cake.Common;

namespace Build.Tasks
{
    [TaskName("OctoDeployTask")]
    public class OctoDeployTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.IsReleasableBranch())
            {
                var server = Environment.GetEnvironmentVariable("octo_server_url") ?? "https://deployments.parkmobile.com/";
                var apiKey = Environment.GetEnvironmentVariable("octo_api_key") ?? context.TeamCity().Environment.Build.ConfigProperties["TeamcityOctoDeployApiKey"];
                var space = Environment.GetEnvironmentVariable("octo_space") ?? "Spaces-22";
                int deployDelayInMinutes = context.EnvironmentVariable<int>("DeployDelayInMinutes", 2);
                var octoProjectName = Environment.GetEnvironmentVariable("octo_project_name") ?? "aaa4-Responder";
                context.OctoCreateRelease(octoProjectName, new CreateReleaseSettings
                {
                    ApiKey = apiKey,
                    Space = space,
                    Server = server,
                    Channel = GetReleaseChannel(context),
                    IgnoreExisting = true,
                    Packages = new Dictionary<string, string> { { "aaa3.basic", context.Options.ApplicationVersion } },
                    DeployAt = DateTime.UtcNow.AddMinutes(deployDelayInMinutes),
                    DeployTo = GetApplicationEnvironment(context),
                    Variables = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("BranchName", context.Options.GitVersion.BranchName) },
                    ForcePackageDownload = true
                });
            }
            else
            {
                context.Information("Skipping deployment, not running on Build server ");
            }
        }

        //We want to only trigger the deploy from team city to preprod and not to prod
        //Prod deploy will happen from Octo in the Release task
        private static string GetApplicationEnvironment(BuildContext context)
        {
            return context.Options.ApplicationEnvironment == "prod" ?
                "preprod" :
                context.Options.ApplicationEnvironment;
        }

        private string GetReleaseChannel(BuildContext context)
        {
            return context.ResolveEnvironmentByBranch();
        }
    }
}
