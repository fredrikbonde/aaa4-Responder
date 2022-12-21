using Amazon;
using Amazon.CodeArtifact;
using Amazon.CodeArtifact.Model;
using Amazon.Runtime;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Common.Tools.DotNet.NuGet.Source;
using Cake.Common.Tools.NuGet;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Threading.Tasks;

namespace Build.Tasks
{
    [TaskName("BCRAwsCodeArtifactsPusherTask")]
    public class AwsCodeArtifactsPusherTask : AsyncFrostingTask<BuildContext>
    {
        public override async Task RunAsync(BuildContext context)
        {
            string awsAccessKey = context.Options.AwsCodeArtifactAwsAccessKey;
            var awsSecretKey = context.Options.AwsCodeArtifactAwsSecret;
            var awsRegion = context.Options.AwsRegion;

            try
            {
                using (var artifactClient = CreateAmazonCodeArtifactClient(awsAccessKey, awsSecretKey, awsRegion, context))
                {
                    var repoUrlResponse = await artifactClient.GetRepositoryEndpointAsync(new GetRepositoryEndpointRequest
                    {
                        Domain = context.Options.AwsCodeArtifactsDomain,
                        DomainOwner = context.Options.AwsCodeArtifactsDomainOwner,
                        Repository = context.Options.AwsCodeArtifactsRepo,
                        Format = PackageFormat.Nuget
                    });
                    var repoUrl = $"{repoUrlResponse.RepositoryEndpoint}v3/index.json";

                    context.Information("Repo url : '{0}'", repoUrl);

                    var tokenResponse = await artifactClient.GetAuthorizationTokenAsync(new GetAuthorizationTokenRequest
                    {
                        Domain = context.Options.AwsCodeArtifactsDomain,
                        DomainOwner = context.Options.AwsCodeArtifactsDomainOwner,
                        DurationSeconds = 900
                    });
                    PushPackages(context, repoUrl, tokenResponse);

                }
            }
            catch (Exception ex)
            {
                context.Error($" Error pushing artifacts, exception :{ex.Message}");
                if ((context.TeamCity().IsRunningOnTeamCity))
                    throw;
            }
        }

        private static void PushPackages(BuildContext context, string repoUrl, GetAuthorizationTokenResponse tokenResponse)
        {
            if (!bool.TryParse(context.Options.AwsCodeArtifactNugetPushSkipDuplicates, out bool skipDuplicates))
                skipDuplicates = true; // set default to true if no argument passed

            
            string sourceName = context.Options.AwsCodeArtifactsSourceName;
            try
            {
                GracefullyRemoveSource(context, repoUrl, sourceName);

                context.DotNetNuGetAddSource(
                    sourceName,
                    new DotNetNuGetSourceSettings
                    {
                        UserName = "aws",
                        Password = tokenResponse.AuthorizationToken, 
                        Source = repoUrl,
                        StorePasswordInClearText = true
                    });

                var packages = context.GetFiles($"{context.Options.ArtifactsFolder}/*.nupkg");
                foreach (var package in packages)
                {
                   context.DotNetNuGetPush(
                   package,
                   new DotNetNuGetPushSettings
                   {
                       Source = sourceName,
                       SkipDuplicate = skipDuplicates 
                   });
                }

            }
            finally
            {
                GracefullyRemoveSource(context, repoUrl, sourceName);
            }
            void GracefullyRemoveSource(BuildContext context, string repoUrl, string sourceName)
            {
                try
                {
                    context.NuGetRemoveSource(
                        sourceName,
                        repoUrl );
                    context.Information("Nuget '{0}' Source removed ", sourceName);
                }
                catch (Exception)
                {
                    context.Information($"Unable to find any package source(s) matching name: {sourceName}");
                }
            }
        }

        private AmazonCodeArtifactClient CreateAmazonCodeArtifactClient(string awsAccessKey, string awsSecretKey, string awsRegion, BuildContext context)
        {
            // add credentials if available , otherwise it AWS will try to authenticate against the origin machine
            if (!string.IsNullOrWhiteSpace(awsAccessKey) && !string.IsNullOrWhiteSpace(awsSecretKey))
            {
                context.Information("Creating AmazonCodeArtifactClient using access key and secret keys ");
                var awsCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
                return new AmazonCodeArtifactClient(awsCredentials, RegionEndpoint.GetBySystemName(awsRegion));
            }
            else
            {
                context.Information("Creating AmazonCodeArtifactClient using the machine IAM role /profile ");
                return new AmazonCodeArtifactClient(RegionEndpoint.GetBySystemName(awsRegion));
            }
        }

    }
}
