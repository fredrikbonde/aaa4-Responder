using System.ComponentModel.DataAnnotations; 
using aaa3.basic.WebApi.Authorization;
using aaa3.basic.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Overleaf.Authentication.AspNetCore.Authorization;

namespace aaa3.basic.WebApi.Controllers
{
    /// <summary>
    /// Management endpoints
    /// </summary>
    [ApiController]
    public class SampleController : Controller
    {
        private readonly ILogger _logger; 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger service</param>
        /// <param name="dataRetentionService">Data retention service</param>
        public SampleController(ILogger<SampleController> logger )
        {
            _logger = logger; 
        }

        /// <summary>
        /// Executes data retention clean up
        /// </summary>
        /// <param name="startingFromDate">The date from it clean up starts, if not passed Now utc will be used, RFC 3339 ISO date time</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Success response</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Client credentials are not valid</response>
        /// <response code="403">Client is not allowed to consume resource with the given X-Api-Key</response>
        /// <response code="500">Server error</response>
        /// <remarks>
        /// Note that the error 400 Bad Request is returned in the following cases:
        ///  - The date is greater than date time UTC Now;
        ///  - Wrong param format;
        /// </remarks>
        [AllowedScopes(ApplicationScopes.SampleScope)]
        [HttpGet]
        [Route("/get")]
        [OpenApiOperation("DataRetentionAsync")]
        [SwaggerResponse(httpStatusCode: 200, typeof(OkObjectResult), Description = "niceeeee")]
        [SwaggerResponse(httpStatusCode: 400, typeof(ErrorResponse), Description = "Bad request")]
        [SwaggerResponse(httpStatusCode: 401, typeof(ErrorResponse), Description = "Client credentials are not valid")]
        [SwaggerResponse(httpStatusCode: 403, typeof(ErrorResponse), Description = "Client is not allowed to consume resource with the given X-Api-Key")]
        [SwaggerResponse(httpStatusCode: 500, typeof(ErrorResponse), Description = "Server error")]
        public virtual async Task<IActionResult> DataRetentionAsync([FromQuery] DateTime? startingFromDate, CancellationToken cancellationToken)
        {
            var logPrefix = $"{nameof(SampleController)}.{nameof(DataRetentionAsync)}() =>";

            _logger.LogInformation($"{logPrefix} startingFromDate: {startingFromDate}"); 

            _logger.LogInformation($"{logPrefix} Execution finished successfully.");

            return Ok("Execution finished successfully.");
        }
    }
}