using Cake.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TestReporting.Client;

namespace Build.TestReporting
{
    public class TestrailService
    {
        private const int PassedTestCode = 1;
        private const int FailedTestCode = 5;
        private readonly TestrailClient _testrailClient;

        public TestrailService(BuildContext context)
        {
            const string Basic = "Basic";
            const string TestRailApiKey = "TestrailApiKey";

            var httpClient = new HttpClient();

            var apiKey = context.EnvironmentVariable<string>(TestRailApiKey, null);

            if (apiKey == null)
                throw new ArgumentNullException($"Could not get key: '{TestRailApiKey}' from environment variables");

            httpClient.BaseAddress = new Uri(context.Options.TestrailBaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic, apiKey);

            _testrailClient = new TestrailClient(httpClient);
        }

        public async Task AddTestCases(int projectId, int sectionId, TestReporting.Xml2CSharp.TestRun trxTests)
        {
            var testCases = await _testrailClient.GetCasesAsync(projectId);

            //if not exists, add case
            foreach (var testCase in trxTests.TestDefinitions.UnitTest)
            {
                if (!testCases.Result.Cases.Exists(x => x.Title == testCase.Name))
                {
                    var theCase = new TestCase() { Title = testCase.Name };

                    var response = await _testrailClient.AddCaseAsync(sectionId, theCase);
                }
            }
        }

        public async Task<int> CreateTestRun(int projectId, string applicationEnvironment, string applicationVersion)
        {
            var run = new TestRun() { Name = $"{applicationEnvironment}-{applicationVersion}-{Guid.NewGuid()}" };
            var testRunResponse = await _testrailClient.CreateTestRunAsync(projectId, run);
            var runId = testRunResponse.Result.Id.GetValueOrDefault();

            return runId;
        }


        public async Task ReportResults(TestReporting.Xml2CSharp.TestRun trxTests, int runId)
        {
            var getTestsResponse = await _testrailClient.GetTestsByRunId(runId);
            var request = new AddResultsRequest() { Results = new List<Result>() };

            //Gather results
            foreach (var runTest in getTestsResponse.Result.Tests)
            {
                var trxResult = trxTests.Results.UnitTestResult
                    .FirstOrDefault(x => x.TestName == runTest.Title);

                // if test does not exist anymore, do not add result
                if (trxResult != null)
                {
                    request.Results.Add(new Result()
                    {
                        TestId = runTest.Id,
                        Comment = trxResult.Output != null ? JsonConvert.SerializeObject(trxResult.Output.ErrorInfo) : "The test has passed",
                        StatusId = trxResult.Outcome == "Passed" ? PassedTestCode : FailedTestCode
                    });
                }
                else
                {
                    //leave empty test cases or try remove testcase?
                }
            }

            //Send results
            await _testrailClient.AddResultsToRun(runId, request);
        }
    }
}
