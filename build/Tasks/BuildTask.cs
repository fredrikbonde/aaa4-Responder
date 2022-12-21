using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Frosting;
using Build.Models;
using Cake.Core.Diagnostics;
using Cake.Common.Tools.DotNet;

namespace Build.Tasks
{
    [TaskName(Constants.TaskBuild)]
    public class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information($"Run build task with build configuration: {context.Options.MsBuildConfiguration}");
            context.DotNetBuild(context.Options.SolutionFile, new DotNetCoreBuildSettings
            {
                Configuration = context.Options.MsBuildConfiguration
            });
        }
    }
}
