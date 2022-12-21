using Build.Models;
using Build.TestReporting;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core.IO;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Build.Tasks
{
    [TaskName(Constants.TaskIntegrationTests)]
    public class IntegrationTestsTask : AsyncFrostingTask<BuildContext>
    {
        public override async Task RunAsync(BuildContext context)
        {
            var applicationEnvironment = context.Environment.GetEnvironmentVariable("Application__AppEnvironment");
            var applicationVersion = context.Environment.GetEnvironmentVariable("Application__Version");

            if (applicationEnvironment == "test" || applicationEnvironment == "sit")
            {
                context.Information("**** running integration tests on release :{0} , on {1} environment ****", applicationVersion, applicationEnvironment);

                try
                {
                    context.CreateDirectory("TestResults");

                    context.DotNetTest(context.Options.IntegrationTestsDll, new DotNetCoreTestSettings
                    {
                        Configuration = context.Options.MsBuildConfiguration,
                        NoBuild = true,
                        Verbosity = DotNetCoreVerbosity.Normal,
                        Loggers = new List<string>() { "trx;LogFileName=testResult.trx" },
                        ResultsDirectory = new DirectoryPath("./TestResults"),
                        EnvironmentVariables = context.EnvironmentVariables()
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    throw;
                }
                finally
                {
                    await ReportResults(context);
                }
            }
            else
            {
                context.Information("**** NOT running integration tests, environment not supported ****");
            }
        }

        private static async Task ReportResults(BuildContext context)
        {
            var projectId = context.Options.TestrailProjectId;
            var sectionId = context.Options.TestrailSectionId;
            var applicationEnvironment = context.Environment.GetEnvironmentVariable("Application__AppEnvironment");
            var applicationVersion = context.Environment.GetEnvironmentVariable("Application__Version");

            if (applicationEnvironment == "test" || applicationEnvironment == "sit")
            {
                context.Information("**** Reporting tests on release :{0} , on {1} environment ****", applicationVersion, applicationEnvironment);

                //create client
                var testrailService = new TestrailService(context);

                //get trx tests
                var trxTests = TrxReader.GetTestRun(context);

                await testrailService.AddTestCases(projectId, sectionId, trxTests);

                context.Information("**** Adding Test Cases.. ****");

                //Create test run
                var runId = await testrailService.CreateTestRun(projectId, applicationEnvironment, applicationVersion);

                context.Information("**** Reporting results.. ****");

                //Report runs
                await testrailService.ReportResults(trxTests, runId);

                context.Information("**** Results Reported! ****");
            }
            else
            {
                context.Information($"**** NOT Reporting tests, environment {applicationEnvironment} not supported *****");
            }
        }
    }
}
