using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace aaa3.basic.Integrations
{
    public static class ClientFactory
    {
        private static ApiClientConfig _config;

        static ClientFactory()
        {
            var env = System.Environment.GetEnvironmentVariable("Application__AppEnvironment");

            if (string.IsNullOrWhiteSpace(env))
                throw new System.Exception("couldn't determine environment, make sure that env variable 'Application__AppEnvironment' is set");
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config/appsettings.json", optional: false)
                .AddJsonFile($"config/appsettings.{env}.json", optional: false)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            _config = config.GetSection(nameof(ApiClientConfig)).Get<ApiClientConfig>();
        }

        public static HttpClient GetClient()
        {
            var client = new HttpClient();

            client.BaseAddress = new System.Uri(_config.BaseUrl);
            client.Timeout = System.TimeSpan.FromMilliseconds(_config.TimeoutMilliseconds);
            client.DefaultRequestHeaders.Add("x-api-key", _config.ApiKey);

            return client;
        }
    }
}