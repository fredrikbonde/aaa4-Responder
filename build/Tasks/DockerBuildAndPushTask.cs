using Amazon;
using Amazon.ECR;
using Amazon.ECR.Model;
using Build.Extensions;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core.Diagnostics;
using Cake.Docker;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Build.Tasks
{
    [TaskName("docker-build-and-push")]
    public class DockerBuildAndPushTask : AsyncFrostingTask<BuildContext>
    {
        private const string MyGetApiKey = "MyGetApiKey";

        public override async Task RunAsync(BuildContext context)
        {
            if (context.IsReleasableBranch())
            {
                context.Log.Information("building and tagging image..");

                string destinationTag = $"{context.Options.EcrRoot}/{context.GetEcrRepoName()}:{context.Options.ApplicationVersion}".ToLower();

                var region = RegionEndpoint.GetBySystemName(context.Options.AwsRegion);

                var ecrClient = new AmazonECRClient(region);
                var getTokenResponse = await ecrClient.GetAuthorizationTokenAsync(new GetAuthorizationTokenRequest());
                if (getTokenResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    var authTokenBytes = Convert.FromBase64String(getTokenResponse.AuthorizationData[0].AuthorizationToken);
                    var authToken = Encoding.UTF8.GetString(authTokenBytes);
                    var decodedTokens = authToken.Split(':');

                    context.DockerLogin(decodedTokens[0], decodedTokens[1], getTokenResponse.AuthorizationData.FirstOrDefault()?.ProxyEndpoint);

                    context.Information($"Image with tag : {destinationTag} ");

                    context.DockerBuild(new DockerImageBuildSettings
                    {
                        File = "Dockerfile",
                        Tag = new string[] { destinationTag },
                        BuildArg = new string[] { $"{MyGetApiKey}={context.EnvironmentVariable<string>(MyGetApiKey, null)}" }
                    }, Directory.GetCurrentDirectory());

                    context.DockerPush(destinationTag);
                    context.DockerRmi(destinationTag);
                }
            }
            else
            {
                context.Information($"Branch `{context.Options?.GitVersion?.BranchName}` is not listed in `ReleasableBranches`, no images will be built");
            }
        }
    }
}
