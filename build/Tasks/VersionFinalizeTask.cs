using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Frosting;
using Cake.Git;
using Build.Models;
using System;

namespace Build.Tasks
{
    [TaskName(Constants.TaskVersionFinalize)]
    public class VersionFinalizeTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            try
            {
                if (context.TeamCity().IsRunningOnTeamCity && context.Options.GitVersion != null
                    && (context.Options.GitVersion.BranchName.StartsWith("release/") || context.Options.GitVersion.BranchName.StartsWith("hotfix/")))
                {
                    var solutionFolder = "./";
                    context.GitTag(solutionFolder, context.Options.ApplicationVersion);
                    context.Information($"Tag {context.Options.ApplicationVersion} created");
                    context.GitPushRef(solutionFolder, context.Options.GitUsername, context.Options.GitPassword, "origin", context.Options.ApplicationVersion);
                    context.Information($"Tag {context.Options.ApplicationVersion} is pushed");
                }
            }
            catch (Exception ex)
            {
                context.Warning($"Error Finalizing GitVersion, Exception:{ex.Message}");
                if (!ex.Message.Contains("tag already exists"))
                    throw;
            }
        }
    }

    [TaskName(Constants.TaskGitTagMaster)]
    public class GitTagMasterTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            try
            {
                if (context.TeamCity().IsRunningOnTeamCity && context.Options.GitVersion != null && (context.Options.GitVersion.BranchName.Equals("master")))
                {
                    var solutionFolder = "./";
                    context.GitTag(solutionFolder, context.Options.ApplicationVersion);
                    context.Information($"Tag {context.Options.ApplicationVersion} created");
                    context.GitPushRef(solutionFolder, context.Options.GitUsername, context.Options.GitPassword, "origin", context.Options.ApplicationVersion);
                    context.Information($"Tag {context.Options.ApplicationVersion} is pushed");
                }
            }
            catch (Exception ex)
            {
                context.Warning($"Error Finalizing GitVersion, Exception:{ex.Message}");
                if (!ex.Message.Contains("tag already exists"))
                    throw;
            }
        }
    }

}
