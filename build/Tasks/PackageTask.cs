using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Pack;
using Cake.Frosting;
using Build.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Build.Tasks
{
    [TaskName(Constants.TaskPackage)]
    public class PackageTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        { 
            foreach (var nuspec in context.Options.PackagingNuspec)
            {
                Pack(context, "pack/Pack.csproj", nuspec, true, true);
            }
        }

        private static void Pack(BuildContext context, string projectFile, string nuspecFile = null, bool noBuild = false, bool noRestore = false)
        {
            var msBuildSettings = new DotNetCoreMSBuildSettings();
            if (!string.IsNullOrWhiteSpace(nuspecFile))
            {
                var nuspecExist = File.Exists(nuspecFile);
                Console.WriteLine($"Nuspec exist: {nuspecExist}");
                msBuildSettings.Properties.Add("NuspecFile", new List<string> { nuspecFile });
                msBuildSettings.Properties.Add("NuspecProperties", new List<string> { $"version={context.Options.ApplicationVersion}" });
            }
            else
            {
                msBuildSettings.Properties.Add("PackageVersion", new List<string> { context.Options.ApplicationVersion });
            }

            Console.WriteLine($"Output directory: {context.Options.ArtifactsFolder}");

            context.DotNetPack(projectFile, new DotNetPackSettings
            {
                OutputDirectory = context.Options.ArtifactsFolder,
                MSBuildSettings = msBuildSettings,
                Configuration = context.Options.MsBuildConfiguration,
                NoBuild = noBuild,
                NoRestore = noRestore
            });
        }
    } 
}
