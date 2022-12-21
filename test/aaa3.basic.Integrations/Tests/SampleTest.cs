using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace aaa3.basic.Integrations.Tests
{
    internal class SampleTest :IntegrationTestBase
    {
        [TestCase(TestName = "calling ping, then the status code should be 200 Ok")]
        public async Task WithCorrectStartDate_ReturnsStatus200()
        {
            //Arrange
            var startingFromDate = DateTime.UtcNow.AddHours(-1);

            //Act
            var createResponse = await _client.GetAsync();

            //Assert
            Assert.That(200, Is.EqualTo(createResponse.StatusCode));
        }
    }
}
