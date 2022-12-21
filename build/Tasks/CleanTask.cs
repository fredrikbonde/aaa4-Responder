using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Build.Models;
using System.IO;

namespace Build.Tasks
{
    [TaskName(Constants.TaskClean)]
    public class CleanTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information("Run clean task");

            string[] dirs = Directory.GetDirectories(@"./src", "bin", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                context.CleanDirectory($"{dir}/{context.Options.MsBuildConfiguration}");
            }
        }
    }
}
