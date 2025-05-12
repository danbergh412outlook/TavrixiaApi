using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Server.Models;
using RestSharp;
using System.Text.Json;

namespace PortfolioApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ApiService _apiService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return _apiService.GetWeatherForecast();
        }
    }
}
