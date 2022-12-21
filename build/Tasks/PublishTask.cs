using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core.IO;
using Cake.Frosting;
using Build.Models;

namespace Build.Tasks
{
    [TaskName(Constants.TaskPublish)]
    public class PublishTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (!context.DirectoryExists(context.Options.ArtifactsFolder))
                context.CreateDirectory(context.Options.ArtifactsFolder);
            foreach (var publishProject in context.Options.PublishProjects)
            {
                if(publishProject.ProjectFile !=null)
                {
                    context.DotNetPublish(publishProject.ProjectFile, new DotNetPublishSettings
                    {
                        Configuration = context.Options.MsBuildConfiguration,
                        NoBuild = true,
                        NoRestore = true,
                        OutputDirectory = $"{context.Options.PublishFolder}/{publishProject.ProjectOutput}"
                    });
                }
                if (publishProject.ZipOutput)
                {
                    if (!string.IsNullOrWhiteSpace(publishProject.SettingsFolder) && !string.IsNullOrWhiteSpace(publishProject.SettingsZipFileName))
                    {
                        context.Zip(context.Options.PublishFolder, new FilePath(context.Options.ArtifactsFolder + "\\" + publishProject.SettingsZipFileName));
                    }
                }
            }
        }
    }
}
