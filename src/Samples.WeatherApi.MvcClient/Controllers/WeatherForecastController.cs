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
        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var content = await client.GetStringAsync("https://localhost:44373/weatherforecast");

            ViewBag.WeatherForecastData = JArray.Parse(content).ToString();

            return View();
        }
    }
}