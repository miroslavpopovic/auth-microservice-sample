using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samples.WeatherApi.WorkerClient
{
    public class Worker : BackgroundService
    {
        private readonly HttpClient _regularHttpClient;
        private readonly IWeatherForecastClient _weatherForecastClient;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IHttpClientFactory factory, IWeatherForecastClient weatherForecastClient, ILogger<Worker> logger)
        {
            _regularHttpClient = factory.CreateClient("weather-api-client");
            _weatherForecastClient = weatherForecastClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTime.Now}");

                var stringResponse = await _regularHttpClient.GetStringAsync("weatherforecast");
                _logger.LogInformation($"Weather API response: {stringResponse}");

                var weatherForecast =
                    (await _weatherForecastClient.GetWeatherForecastAsync()).ToArray();

                _logger.LogInformation(
                    $"Downloaded {weatherForecast.Length} forecasts; " +
                    $"max temp: {weatherForecast.Max(x => x.TemperatureC)}; " +
                    $"min temp: {weatherForecast.Min(x => x.TemperatureC)}");

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
