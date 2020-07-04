using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Samples.WeatherApi.WorkerClient
{
    public class WeatherForecastClient : IWeatherForecastClient
    {
        private readonly HttpClient _httpClient;

        public WeatherForecastClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
        {
            var response = await _httpClient.GetStringAsync("weatherforecast");

            return JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(
                response, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
        }
    }

    public interface IWeatherForecastClient
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
    }
}
