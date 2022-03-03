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
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogWarning("Yeah I have logged 1 1 1");

        

            foreach (var header in Request.Headers)
            {
                _logger.LogWarning($"The header values are {header.Key}-{header.Value.ToString()}");
            }
            Random rnd = new Random();
            var randNumber = rnd.Next(0, 100);
            if (randNumber < 12)
            {
                throw new ApplicationException("@@@some application exception");
            }
            

            await _httpClient.GetAsync("https://www.google.com/");


            await _httpClient.GetAsync("https://aspnetcore2nachi.azurewebsites.net/WeatherForecast/permissions");

            if (randNumber < 20)
            {
                throw new Exception("@@@some exception");
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
