using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace aspnetcoreserver.Controllers
{
    [ApiController]
    [Route("/api/v2/subjobs")]
    public class SubJobsController : ControllerBase
    {
        private readonly ILogger<SubJobsController> _logger;
        private readonly HttpClient _httpClient;

        public SubJobsController(ILogger<SubJobsController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<string> SubJobs()
        {
            _logger.LogWarning("Subjobs controller called");
            foreach (var header in Request.Headers)
            {
                _logger.LogWarning($"The header values are {header.Key}-{header.Value.ToString()}");
            }
            Random rnd = new Random();
            
            var randNumber = rnd.Next(0, 100);
            _logger.LogWarning($"Random number is {randNumber}");
            if (randNumber < 12)
            {
                throw new ApplicationException("@@@some application exception");
            }
            if (randNumber < 20)
            {
                throw new Exception("@@@some exception");
            }

            var r1 = _httpClient.GetAsync("https://aspnetcore2nachi.azurewebsites.net/api/v1/jobs/permissions");
            var r2 = _httpClient.GetAsync("https://www.google.com/");
            var r3 = _httpClient.GetAsync("https://news.google.com/");

            await Task.WhenAll(r1, r2, r3);

            if (!r1.Result.IsSuccessStatusCode) throw new Exception("Call to permissions failed");
            if (!r2.Result.IsSuccessStatusCode) throw new Exception("google failed");
            if (!r3.Result.IsSuccessStatusCode) throw new Exception("google news failed");

            return "subjobs";
        }
    }
}
