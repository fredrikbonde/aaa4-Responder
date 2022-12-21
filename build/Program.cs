using Build;
using Build.Tasks;
using Cake.Core;
using Cake.Frosting;
using Cake.VulnerabilityScanner;
using dotenv.net;
using System.Threading;
using System.Threading.Tasks;

public static class Program
{
    public static int Main(string[] args)
    {
        DotEnv.Fluent()
              .WithEnvFiles("../.env")
              .Load();

        return new CakeHost()
            .UseWorkingDirectory("..")
            .InstallTools()
            .UseContext<BuildContext>()
            .Run(args);
    }
}
[TaskName("scan pacakges")]
public sealed class ScanPackagesTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        // SonaType token,  base64 username:password
        var ossIndexToken = context.Environment.GetEnvironmentVariable("OSS_INDEX_TOKEN");
        await context.ScanPackagesAsync(new ScanPackagesSettings
        {
            Ecosystem = "nuget",
            FailOnVulnerability = true,
            OssIndexBaseUrl = "https://ossindex.sonatype.org/",
            OssIndexToken = ossIndexToken,
            SolutionFile = context.Options.SolutionFile,
            Verbosity = Microsoft.Extensions.Logging.LogLevel.Debug,

        }, CancellationToken.None);
    }
}




[TaskName("Default")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(VersionTask))]
[IsDependentOn(typeof(SonarInitTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(ScanPackagesTask))]
[IsDependentOn(typeof(TestAndCoverTask))]
[IsDependentOn(typeof(PublishTask))]
[IsDependentOn(typeof(SonarEndTask))]
[IsDependentOn(typeof(PackageTask))]
[IsDependentOn(typeof(AwsCodeArtifactsPusherTask))]
[IsDependentOn(typeof(EcrDeployBuildTask))]
[IsDependentOn(typeof(DockerBuildAndPushTask))]
[IsDependentOn(typeof(OctoDeployTask))]
public class DefaultTask : FrostingTask
{
}

[TaskName(nameof(VerifyAndReleaseNext))]
[IsDependentOn(typeof(ParameterStoreUploadTask))]
[IsDependentOn(typeof(ServerlessDeployTask))]
[IsDependentOn(typeof(ReleaseTask))]
public sealed class VerifyAndReleaseNext : FrostingTask
{
}


//[IsDependentOn(typeof(DbMigrationTask))]
//public sealed class DbMigration : FrostingTask
//{
//}
