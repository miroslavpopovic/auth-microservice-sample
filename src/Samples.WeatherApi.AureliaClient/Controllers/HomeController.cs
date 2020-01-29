using Microsoft.AspNetCore.Mvc;

namespace Samples.WeatherApi.AureliaClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}