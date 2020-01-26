using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Samples.WeatherSummaryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherSummaryController : ControllerBase
    {
        private readonly HttpClient _client;

        public WeatherSummaryController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("weather-api-client");
        }

        [HttpGet]
        public async Task<WeatherSummary> Get()
        {
            // This is using an existing access_token, to also call another API
            // It needs to include another API scope too

            var accessToken = HttpContext.Request.Headers["Authorization"][0].Replace("Bearer ", string.Empty);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await _client.GetStringAsync(string.Empty);

            var forecasts = JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(
                content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new WeatherSummary {Forecasts = forecasts};
        }
    }
}
