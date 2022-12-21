using Cake.Common.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Sonar;
using Build.Models;

namespace Build.Tasks
{
    [TaskName(Constants.TaskSonarInit)]
    public class SonarInitTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var dotCoverReportsPath = new FilePath(context.Options.CoverageReport).FullPath;
            var pullRequestNumber = context.Options.PullRequestNumber;
            var pullRequestSourceBranch = context.Options.PullRequestSourceBranch;
            var pullRequestTargetBranch = context.Options.PullRequestTargetBranch;
            var projectName = string.IsNullOrWhiteSpace(context.Options.SonarProjectName)
                ? context.Options.ApplicationName
                : context.Options.SonarProjectName;

            //check if its local deployment
            if (!string.IsNullOrWhiteSpace(context.Options.SonarQubeUrlForLocalDeployment) && !string.IsNullOrWhiteSpace(context.Options.SonarQubeLoginForLocalDeployment))
            {
                context.Information("Start SonarQube for local deployment");
                context.SonarBegin(new SonarBeginSettings
                {
                    Url = context.Options.SonarQubeUrlForLocalDeployment,
                    Login = context.Options.SonarQubeLoginForLocalDeployment,
                    Verbose = true,
                    Key = context.Options.SonarQubeProjectKey,
                    Name = projectName,
                    Version = context.Options.ApplicationVersion,
                    DotCoverReportsPath = dotCoverReportsPath,
                    Branch = context.Options.GitVersion.BranchName
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(context.Options.SonarServerUrl))
            {
                context.Information("Skipping Sonar integration since url is not specified");
                return;
            }

            var isPullRequest = context.Options.GitVersion.BranchName.ToLower().Contains("pull-request") && !context.Options.GitVersion.BranchName.ToLower().Contains("merge");
            context.Information($"is a PR: {isPullRequest}");

            if (isPullRequest)
            {
                context.Information("PR Number: " + pullRequestNumber);
                context.Information("PR source branch: " + pullRequestSourceBranch);
                context.Information("PR target branch: " + pullRequestTargetBranch);
                if (int.TryParse(pullRequestNumber, out int prNumber))
                {
                    context.SonarBegin(new SonarBeginSettings
                    {
                        Url = context.Options.SonarServerUrl,
                        Login = context.Options.SonarServerLogin,
                        Verbose = false,
                        Key = context.Options.SonarQubeProjectKey,
                        Name = projectName,
                        Version = context.Options.ApplicationVersion,
                        DotCoverReportsPath = dotCoverReportsPath,
                        PullRequestBranch = pullRequestSourceBranch,
                        PullRequestBase = pullRequestTargetBranch,
                        PullRequestKey = prNumber
                    });
                }
                else
                {
                    context.Warning($"Skipping pull request analysis , PR number : {pullRequestNumber} couldn't be parsed to an integer");
                }
            }
            else
            {
                context.SonarBegin(new SonarBeginSettings
                {
                    Url = context.Options.SonarServerUrl,
                    Login = context.Options.SonarServerLogin,
                    Verbose = false,
                    Key = context.Options.SonarQubeProjectKey,
                    Name = projectName,
                    Version = context.Options.ApplicationVersion,
                    DotCoverReportsPath = dotCoverReportsPath,
                    Branch = context.Options.GitVersion.BranchName
                });
            }
        }
    }

    [TaskName(Constants.TaskSonarEnd)]
    public class SonarEndTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (!CheckSonarUp(context.Options.SonarServerUrl))
            {
                context.Information("Skipping Sonar integration since server is not reachable");
                return;
            }

            if (!string.IsNullOrWhiteSpace(context.Options.SonarQubeLoginForLocalDeployment))
            {
                context.SonarEnd(new SonarEndSettings { Login = context.Options.SonarQubeLoginForLocalDeployment });
            }

            context.SonarEnd(new SonarEndSettings { Login = context.Options.SonarServerLogin });
        }

        private bool CheckSonarUp(string url)
        {
            try
            {
                var version = new SonarServer().GetVersion(url).Result;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
