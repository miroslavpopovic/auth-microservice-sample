using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Samples.WeatherApi.MvcClient.Controllers
{
    public class WeatherForecastController : Controller
    {
        private const string WeatherForecastApiUrl = "https://localhost:44373/weatherforecast";
        private const string WeatherSummaryApiUrl = "https://localhost:44303/weathersummary";
        private readonly HttpClient _forecastClient;
        private readonly HttpClient _summaryClient;

        public WeatherForecastController(IHttpClientFactory clientFactory)
        {
            _forecastClient = clientFactory.CreateClient("weather-api-client");
            _summaryClient = clientFactory.CreateClient("weather-summary-api-client");
        }

        public async Task<IActionResult> Index()
        {
            // This is using an existing access_token, used to authorize access to MVC app, to also call API
            // It needs to include API scope too

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync(WeatherForecastApiUrl);

            ViewBag.WeatherForecastData = JArray.Parse(content).ToString();

            return View();
        }

        public async Task<IActionResult> Index2()
        {
            // This is using a separate API Client to get access token
            // for accessing Weather API using the Client Credentials

            var content = await _forecastClient.GetStringAsync(WeatherForecastApiUrl);

            ViewBag.WeatherForecastData = JArray.Parse(content).ToString();

            return View();
        }

        public async Task<IActionResult> Summary()
        {
            // This is using a separate API Client to get access token
            // for accessing Weather API using the Client Credentials

            var content = await _summaryClient.GetStringAsync(WeatherSummaryApiUrl);

            ViewBag.WeatherSummaryData = JObject.Parse(content).ToString();

            return View();
        }
    }
}
