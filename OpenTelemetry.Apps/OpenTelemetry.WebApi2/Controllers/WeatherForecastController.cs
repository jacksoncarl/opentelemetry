using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenTelemetry.WebApi2.Controllers
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
        private IDistributedCache _cache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }


        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            _logger.LogInformation($"Logging current activity: {JsonSerializer.Serialize(Activity.Current)}");

            IEnumerable<WeatherForecast> weatherForecast = null;
            var cachedWeatherForecast = await _cache.GetStringAsync("weatherforecast");
            if (string.IsNullOrEmpty(cachedWeatherForecast))
            {
                var rng = new Random();
                weatherForecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

                await _cache.SetStringAsync("weatherforecast",
                    JsonSerializer.Serialize(weatherForecast),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
                    });
            }
            else
            {
                weatherForecast = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedWeatherForecast);
            }

            return weatherForecast;
        }
    }
}
