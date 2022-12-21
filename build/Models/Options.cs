using Cake.Common.Tools.GitVersion;
using System.Collections.Generic;

namespace Build.Models
{
    public class Options
    {
        public GitVersion GitVersion { get; set; }

        public string MsBuildConfiguration { get; set; }
        public string DotnetRuntime { get; set; }
        public string Framework { get; set; }

        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationPlatfrom { get; set; }
        public string ApplicationEnvironment { get; set; }
        public string ApplicationSystem { get; set; }
        public string ApplicationSubsystem { get; set; }
        public string ApplicationOwner { get; set; }

        public string ToolsFolder { get; set; }
        public string ArtifactsFolder { get; set; }
        public string CoverageResult { get; set; }
        public string CoverageReport { get; set; }
        public string PublishFolder { get; set; }
        public string SolutionFile { get; set; }

        public string TestProjectPattern { get; set; }
        public List<string> PackagingNuspec { get; set; }
        public List<string> DotCoverFilters { get; set; }

        public List<PublishProject> PublishProjects { get; set; }

        public string GitUsername { get; set; }
        public string GitPassword { get; set; }
        public string GitRepoUrl { get; set; }
        public string GitRepoRelativePath { get; set; }

        //sonarqube
        public string SonarQubeProjectKey { get; set; }

        public string SonarServerUrl { get; set; }
        public string SonarServerLogin { get; set; }
        public string SonarProjectName { get; set; }
        public string PullRequestNumber { get; set; }
        public string PullRequestSourceBranch { get; set; }
        public string PullRequestTargetBranch { get; set; }

        //sonarqube for local deployment
        public string SonarQubeUrlForLocalDeployment { get; set; }
        public string SonarQubeLoginForLocalDeployment { get; set; }

        public string AwsProfile { get; set; }
        public string AwsRegion { get; set; }

        //for code artifact
        public string AwsCodeArtifactsRepo { get; set; }
        public string AwsCodeArtifactsDomain { get; set; }
        public string AwsCodeArtifactsSourceName { get; set; }
        public string AwsCodeArtifactsDomainOwner { get; set; }
        public string AwsCodeArtifactNugetPushSkipDuplicates { get; set; }
        public string AwsCodeArtifactAwsAccessKey { get; set; }
        public string AwsCodeArtifactAwsSecret { get; set; }

        //ECR
        public string EcrRoot { get; set; }

        // Branches
        public string BitBucketProjectKey { get; set; }
        public string BitBucketRepoSlug { get; set; }
        public List<string> ReleasableBranches { get; set; }
        public Dictionary<string,string> BranchEnvironmentMap { get; set; }

        // IntegrationTests & Reporting
        public string IntegrationTestsDll { get; set; }
        public int TestrailProjectId { get; set; }
        public int TestrailSectionId { get; set; }
        public string TestrailBaseUrl { get; set; }

        public class PublishProject
        {
            public string ProjectFile { get; set; }
            public string ProjectOutput { get; set; }
            public bool ZipOutput { get; set; }
            public string ZipFileName { get; set; }
            public string SettingsFolder { get; set; }
            public string SettingsZipFileName { get; set; }
        }
    }
}
