using System.Net;
using aaa3.basic.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace aaa3.basic.WebApi.Controllers
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ApiController" />
    [Route("app/ping")]
    [AllowAnonymous]
    public class MonitoringController : Controller
    {
        private readonly IConfiguration _configuration;

        public MonitoringController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///     This resource has the sole purpose of indicating whether the api is to be regarded as "alive", meaning the URL is
        ///     available and can be reached.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        #region swagger documentation 
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(PingResponse))]
        #endregion

        public IActionResult Ping()
        {
            var version = Environment.GetEnvironmentVariable("Application__Version");
            return new OkObjectResult(string.IsNullOrEmpty(version)
                    ? new PingResponse()
                    : new PingResponse
                    {
                        Version = version,
                    });
        }
    }
}