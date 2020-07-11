using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiDemo.Data
{
    public interface IWeatherForecastData
    {
        IEnumerable<WeatherForecast> GetData(int a = 0);
    }

    public class WeatherForecastData : IWeatherForecastData
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public IEnumerable<WeatherForecast> GetData(int a = 0)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
