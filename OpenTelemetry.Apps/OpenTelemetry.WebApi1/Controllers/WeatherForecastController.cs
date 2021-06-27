using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenTelemetry.WebApi1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private static readonly ActivitySource _activity = new ActivitySource("webapi1.WeatherForecastController");

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation($"Calling App3: {_configuration["Api2Endpoint"]}");

            _logger.LogInformation($"Logging current activity: {JsonSerializer.Serialize(Activity.Current)}");

            using (var activity = _activity.StartActivity("webapi2.http.fetch"))
            {
                activity?.SetTag("foo", 1);

                Activity.Current?.AddEvent(new ActivityEvent("Something happened."));

                var response = await _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>(_configuration["Api2Endpoint"]);

                return new OkObjectResult(response);
            }
        }
    }
}
