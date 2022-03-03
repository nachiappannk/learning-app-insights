using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace aspnetcore2.Controllers
{
    [ApiController]
    [Route("/api/v1/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly HttpClient _httpClient;

        public JobsController(ILogger<JobsController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("permissions")]
        public async Task<string> GetPermissions() 
        {
            _logger.LogWarning("Permissions called");
            Random rnd = new Random();
            var randNumber = rnd.Next(100, 1000);
            _logger.LogInformation($"The random number is {randNumber}");
            await Task.Delay(randNumber);
            if (randNumber < 12)
            {
                throw new NotImplementedException("@99 some application exception");
            }
            if (randNumber < 20)
            {
                throw new ArrayTypeMismatchException("@99 some exception");
            }
            return "permissions";
        }


        [HttpGet]
        public async Task<string> GetJobs()
        {
            _logger.LogWarning("Getting Jobs");
            foreach (var header in Request.Headers)
            {
                _logger.LogDebug($"The header values are {header.Key}-{header.Value.ToString()}");
            }

            Random rnd = new Random();
            var randNumber = rnd.Next(0, 100);
            if (randNumber < 12)
            {
                throw new ApplicationException("### some application exception");
            }
            if (randNumber < 20)
            {
                throw new Exception("### some exception");
            }

            var response = await _httpClient.GetAsync("https://aspnetcoreserver1.azurewebsites.net/WeatherForecast");
            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new NotSupportedException("Dependency failed");
            }
            return "jobs";
        }
    }
}
