using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.GitVersion;
using Cake.Frosting;
using Build.Models;
using System;
using Build.Extensions;

namespace Build.Tasks
{
    [TaskName(Constants.TaskGitVersion)]
    public class VersionTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            try
            {
                CalculateVersion();
            }
            catch (Exception ex)
            {
                context.Warning($"Error Calculating GitVersion, Retry is in progress, Exception:{ex.Message}");
                CalculateVersion();
            }
            finally
            {
                if (context.TeamCity().IsRunningOnTeamCity)
                {
                    // Set the build number in teamcity.
                    context.Information("Setting teamcity version using NuGetVersionV2");
                    context.TeamCity().SetBuildNumber(context.Options.ApplicationVersion);
                }
            }
            void CalculateVersion()
            {
                var version = context.GitVersion(new GitVersionSettings
                {
                    Verbosity = GitVersionVerbosity.Diagnostic,
                    NoFetch = false
                });
                Console.WriteLine($"Version : {version.NuGetVersionV2}");
                context.Options.ApplicationVersion = version.NuGetVersionV2;
                context.Options.GitVersion = version;
                SetBuildEnvironment(context);  
            }
        }
        private void SetBuildEnvironment(  BuildContext context)
        {
            var environment = context.ResolveEnvironmentByBranch();
            if (environment != null)
            {
                Console.WriteLine($"Environment has been set from branch map to : {environment}");
                context.Options.ApplicationEnvironment = environment;
                Environment.SetEnvironmentVariable("Application__AppEnvironment", environment);
                Environment.SetEnvironmentVariable("Application__Environment", environment);
            }
        }

    }
}
