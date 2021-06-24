using System;

namespace OpenTelemetry.WebApi1
{
    public record WeatherForecast
    {
        public DateTime Date { get; init; }

        public int TemperatureC { get; init; }

        public int TemperatureF { get; init; }

        public string Summary { get; init; }
    }
}
