using aaa3.basic.WebApi.Extensions;
using Overleaf.Configuration.HealthCheck.Controllers;
using aaa3.basic.WebApi.Monitoring;

namespace aaa3.basic.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           // SetPasswordToConnectionstring(Configuration);
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder
            .AddConfiguration(Configuration)
            .AddConsole());

            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "aaa3.basic";
                configure.Version = System.Environment.GetEnvironmentVariable("Application__Version") ?? "v1";
                configure.AddBearerToken();
                configure.AddXapiKey();
                configure.AddTripId();
            }); 
            services.AddMvc()
                .AddApplicationPart(typeof(HealthCheckController).Assembly);

            services.AddHealthChecks(); 
            services.AddSingleton<IMonitoring, NewRelicMonitoring>(); 
            services.AddDistributedMemoryCache();
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if(env.EnvironmentName.ToLower() != "prod")
            {
                app.UseOpenApi();
                app.UseSwaggerUi3(c => { c.Path = "/swagger"; });
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHealthChecks("/health");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        //private static void SetPasswordToConnectionstring(IConfiguration settings)
        //{
        //    var connstring = settings.GetSection("ConnectionStrings").Get<ConnectionStrings>();
        //    var password = settings.GetValue<string>("ConnectionStrings:Enforcement:Password");

        //    if (!string.IsNullOrWhiteSpace(password))
        //    {
        //        settings["ConnectionStrings:Enforcement:Default"] = string.Format(connstring.Enforcement.Default, password);
        //    }
        //}
    }
}
