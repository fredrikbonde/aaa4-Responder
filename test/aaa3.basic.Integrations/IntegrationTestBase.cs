 
using aaa3.basic.Integrations.Tests;
using aaa3.basic.WebClient;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace aaa3.basic.Integrations
{
    public abstract class IntegrationTestBase
    {
        protected readonly DateTime ExpiredDate = DateTime.UtcNow.AddYears(-10);
        protected SampleClient _client;

        [OneTimeSetUp]
        public void Test()
        {
            _client = new SampleClient(ClientFactory.GetClient()); 
        }
    }
}
