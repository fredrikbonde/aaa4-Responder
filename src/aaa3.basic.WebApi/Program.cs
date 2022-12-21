using System.Text;
using Amazon.SimpleSystemsManagement.Model;
using Overleaf.Authentication.AspNetCore.Extensions;
using Overleaf.Logging;

namespace aaa3.basic.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseAuthentication();
                    webBuilder.UseStartup<Startup>(); 
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var path = GetParametersPath();
                        var filter =
                            $"{Environment.GetEnvironmentVariable("Application__Environment")}-{Environment.GetEnvironmentVariable("Application__Version")}";


                        bool.TryParse(Environment.GetEnvironmentVariable("USE_LOCAL_CONFIG"), out bool useLocalConfig);

                       
                        if (useLocalConfig)
                        {
                            Console.WriteLine($"using local config ..");
                            config.AddJsonFile("configs/appsettings.test.json", false);
                        }
                        else
                        {
                            Console.WriteLine($"Parameter store path : '{path}', Filter: '{filter}'");
                            config.AddSystemsManager(configurationSource =>
                            {
                                configurationSource.Path = path;
                                configurationSource.Optional = false;
                                configurationSource.Filters.Add(new ParameterStringFilter
                                {
                                    Key = "Label",
                                    Option = "Equals",
                                    Values = new List<string> { filter }
                                });
                            });
                        } 
 
                    });

                });

        private static string GetParametersPath()
        {
            var sb = new StringBuilder();
            sb.Append("/");
            sb.Append(Environment.GetEnvironmentVariable("Application__Environment"));
            sb.Append("/");
            sb.Append(Environment.GetEnvironmentVariable("Application__Platform"));
            sb.Append("/");
            sb.Append(Environment.GetEnvironmentVariable("Application__System"));
            sb.Append("/");
            sb.Append(Environment.GetEnvironmentVariable("Application__Subsystem"));
            sb.Append("/");
            sb.Append("app");
            return sb.ToString().ToLower();
        }
    }
}

