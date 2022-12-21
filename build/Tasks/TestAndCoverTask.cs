using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Frosting;
using Build.Models;
using System;
using Cake.Common.Tools.DotCover;
using Cake.Core.IO;
using Cake.Common.Tools.DotCover.Cover;
using Cake.Common.Build;
using Cake.Common.Tools.DotCover.Report;

namespace Build.Tasks
{
    [TaskName(Constants.TaskTestAndCover)]
    public class TestAndCoverTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {


            context.DotNetTest($"./{context.Options.SolutionFile}", new DotNetCoreTestSettings
            {
                Configuration = context.Options.MsBuildConfiguration,
                NoBuild = true,
                Verbosity = DotNetCoreVerbosity.Normal,
                Filter = @"FullyQualifiedName!~Integrations"
            });
            //var coverSettings = new DotCoverCoverSettings();
            //coverSettings.Filters.Add("-:Tests");
            //coverSettings.Filters.Add("-:build");

            //try
            //{
            //    context.DotCoverCover(
            //               tool => tool.DotNetTest($"./{context.Options.SolutionFile}", new DotNetCoreTestSettings
            //               {
            //                   Configuration = context.Options.MsBuildConfiguration,
            //                   NoBuild = true,
            //                   Verbosity = DotNetCoreVerbosity.Normal,
            //                   Filter = @"FullyQualifiedName!~Integrations"
            //               }), 
            //               new FilePath(context.Options.CoverageResult),
            //               coverSettings);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    throw;
            //}

            //finally
            //{
            //    if (context.TeamCity().IsRunningOnTeamCity)
            //    {
            //        context.TeamCity().ImportDotCoverCoverage(context.Options.CoverageResult);
            //    }
            //    context.DotCoverReport(context.Options.CoverageResult, new FilePath(context.Options.CoverageReport), new DotCoverReportSettings
            //    {
            //        ReportType = DotCoverReportType.HTML
            //    });
            //}
        }
    }
}
