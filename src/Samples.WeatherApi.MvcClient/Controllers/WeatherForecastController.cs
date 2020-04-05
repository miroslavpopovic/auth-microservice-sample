using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Samples.WeatherApi.MvcClient.Controllers
{
    public class WeatherForecastController : Controller
    {
        private readonly HttpClient _forecastClient;
        private readonly HttpClient _summaryClient;
        private readonly string _weatherForecastApiUrl;
        private readonly string _weatherSummaryApiUrl;

        public WeatherForecastController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            var urlPrefix = configuration.GetValue<string>("ApplicationUrlPrefix");

            _weatherForecastApiUrl = $"{urlPrefix}:44373/weatherforecast";
            _weatherSummaryApiUrl = $"{urlPrefix}:44303/weathersummary";

            _forecastClient = clientFactory.CreateClient("weather-api-client");
            _summaryClient = clientFactory.CreateClient("weather-summary-api-client");
        }

        public async Task<IActionResult> Index()
        {
            // This is using an existing access_token, used to authorize access to MVC app, to also call API
            // It needs to include API scope too

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync(_weatherForecastApiUrl);

            ViewBag.WeatherForecastData = JArray.Parse(content).ToString();

            return View();
        }

        public async Task<IActionResult> Index2()
        {
            // This is using a separate API Client to get access token
            // for accessing Weather API using the Client Credentials

            var content = await _forecastClient.GetStringAsync(_weatherForecastApiUrl);

            ViewBag.WeatherForecastData = JArray.Parse(content).ToString();

            return View();
        }

        public async Task<IActionResult> Summary()
        {
            // This is using a separate API Client to get access token
            // for accessing Weather API using the Client Credentials

            var content = await _summaryClient.GetStringAsync(_weatherSummaryApiUrl);

            ViewBag.WeatherSummaryData = JObject.Parse(content).ToString();

            return View();
        }
    }
}
