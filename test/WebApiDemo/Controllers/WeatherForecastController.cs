using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebApiDemo.Data;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastData weatherForecast;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastData weatherForecast)
        {
            _logger = logger;
            this.weatherForecast = weatherForecast;
        }

        [HttpGet]
        public virtual IEnumerable<WeatherForecast> Get(int a = 0)
        {
            return weatherForecast.GetData(a);
        }
    }
}