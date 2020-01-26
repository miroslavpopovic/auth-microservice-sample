using System.Collections.Generic;
using System.Linq;

namespace Samples.WeatherSummaryApi
{
    public class WeatherSummary
    {
        public IEnumerable<WeatherForecast> Forecasts { get; set; }

        public int MaxTemperatureC => Forecasts.Max(x => x.TemperatureC);

        public int MinTemperatureC => Forecasts.Min(x => x.TemperatureC);
    }
}
