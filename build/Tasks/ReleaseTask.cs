using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.OctopusDeploy;
using Cake.Frosting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Build.Tasks
{
    [TaskName("Release Task")]
    [IsDependentOn(typeof(IntegrationTestsTask))]
    public class ReleaseTask : AsyncFrostingTask<BuildContext>
    {
        private const string masterHeadRef = "refs/heads/main";
        private const string devHeadRef = "refs/heads/development";
        private static HttpClient _httpClient = new HttpClient();
        public override async Task RunAsync(BuildContext context)
        {
            var applicationEnvironment = context.Environment.GetEnvironmentVariable("Application__Environment");
            if (applicationEnvironment != "prod")
            {
                var bitBucketRestApiUrl = context.Environment.GetEnvironmentVariable("BitBucketRestApiUrl") ?? "https://stash.parkmobile.com/rest/api/1.0/";
                var applicationVersion = context.Environment.GetEnvironmentVariable("Application__Version");
                var bitBucketToken = context.Environment.GetEnvironmentVariable("BitBucketToken");

                InitBitBucketClient(bitBucketRestApiUrl, bitBucketToken);

                var bitbucketClient = new BitbucketClient(_httpClient);
                if (applicationEnvironment == "test")
                {
                    try
                    {
                        context.Information("**** creating release branch ****");
                        await bitbucketClient.CreateBranchAsync(new Branch
                        {
                            Name = GetReleaseBranchName(applicationVersion),
                            StartPoint = devHeadRef
                        }, context.Options.BitBucketProjectKey, context.Options.BitBucketRepoSlug); ;
                    }
                    catch (System.Exception ex)
                    {
                        context.Error("Error Creating a branch, Error {0}", ex.Message);
                        throw;
                    }
                }

                if (applicationEnvironment == "sit")
                {
                    try
                    {
                        context.Information("**** creating pull request from release to master ****");

                        var branchName = GetReleaseBranchName(applicationVersion);
                        
                        var pr = await bitbucketClient.CreatePullRequestAsync(new PullRequest
                        {
                            Title = $"CD created PR, { branchName } to master",
                            FromRef = new Ref
                            {
                                Id = $"refs/heads/{branchName}"
                            },
                            ToRef = new Ref
                            {
                                Id = masterHeadRef
                            }
                        }, context.Options.BitBucketProjectKey, context.Options.BitBucketRepoSlug);

                        context.Information("**** merging pull request number {0} ****", pr.Id);
                        await bitbucketClient.MergePullRequestAsync(context.Options.BitBucketProjectKey, context.Options.BitBucketRepoSlug, pr.Id, "0");

                        context.Information("**** tagging master branch ****", pr.Id);
                        await bitbucketClient.CreateTagAsync(new Branch
                        {
                            StartPoint = masterHeadRef,
                            Name = applicationVersion.Split('-')[0],
                        }, context.Options.BitBucketProjectKey, context.Options.BitBucketRepoSlug);


                    }
                    catch (System.Exception ex)
                    {
                        context.Error("Error creating PR, merge or tag, Error {0}", ex.Message);
                        throw;
                    }
                }

                var promoteToProd = context.EnvironmentVariable<bool>("PromoteToProd", false);
                if (applicationEnvironment == "preprod" && promoteToProd)
                {
                    var octoServer = Environment.GetEnvironmentVariable("octo_server_url") ?? "https://deployments.parkmobile.com/";
                    var apiKey = Environment.GetEnvironmentVariable("octo_api_key");
                    var space = Environment.GetEnvironmentVariable("octo_space") ?? "Spaces-22";
                    var octoProjectName = Environment.GetEnvironmentVariable("octo_project_name") ?? "Enforcement-Register";
                    context.OctoPromoteRelease(octoServer, apiKey, octoProjectName, applicationEnvironment, "prod", new OctopusDeployPromoteReleaseSettings
                    {
                        DeployAt = DateTime.UtcNow.AddMinutes(2),
                        WaitForDeployment = false,
                        Space = space,
                    });

                }
            }
        }

        private string GetReleaseBranchName(string applicationVersion)
        {
            string branch = applicationVersion.Split('-')[0];
            return $"release/{branch}";
        }

        private static void InitBitBucketClient(string bitBucketRestApiUrl, string bitBucketToken)
        {
            _httpClient.BaseAddress = new System.Uri(bitBucketRestApiUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bitBucketToken);
        }
    }
}
