namespace Build.Models
{
    public static class Constants
    {
        public const string TaskGitVersion = "GitVersionTask";
        public const string TaskClean = "CleanTask";
        public const string TaskBuild = "BuildTask";
        public const string TaskSonarInit = "SonarInitTask";
        public const string TaskSonarEnd = "SonarEndTask";
        public const string TaskTestAndCover = "TestAndCoverTask";
        public const string TaskPublish = "PublishTask";
        public const string TaskPackage = "PackageTask";
        public const string TaskVersionFinalize = "VersionFinalizeTask";
        public const string TaskGitTagMaster = "GitTagMaster";
        public const string TaskServerlessDeploy = "ServerlessDeployTask";
        public const string TaskIntegrationTests = "IntegrationTestsTask";
        public const string TaskReportResults = "ReportTestResultsTask";

        public const string ApplicationEnvironment = "Application__Environment";
        public const string SonarServerUrl = "SonarQube.ServerUrl";
        public const string SonarServerLogin = "SonarQube.Login";
        public const string SonarProjectName = "SonarQube.ProjectName";
        public const string PullRequestSourceBranch = "PullRequestSourceBranch";
        public const string PullRequestTargetBranch = "PullRequestTargetBranch";
        public const string PullRequestNumber = "PullRequestNumber";

        public const string CodeArtifactNugetPushSkipDuplicates = "CodeArtifactNugetPushSkipDuplicates";
        public const string CodeArtifactAwsAccessKeyPhonixx = "CodeArtifactAwsAccessKeyPhonixx";
        public const string CodeArtifactAwsSecretPhonixx = "CodeArtifactAwsSecretPhonixx";
        public const string CodeArtifactAwsAccessKeyBloxx = "CodeArtifactAwsAccessKeyBloxx";
        public const string CodeArtifactAwsSecretBloxx = "CodeArtifactAwsSecretBloxx";

        public const string GitGenericUserName = "GitGenericUserNameNLBuild";
        public const string GitGenericUserPassword = "GitGenericUserPasswordNLBuild";
        public const string AwsProfile = "AWS_PROFILE";

    }
}
