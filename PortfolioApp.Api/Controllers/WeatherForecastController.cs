using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace PortfolioApp.Api.Controllers
{
    //[Authorize(Roles = "Role.All")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]    //[Authorize(Roles = "Role.All")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseApiController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "Lava"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var secretValue2 = _configuration["AllowedHosts"];
            var secretValue = _configuration["ConnectionStrings--TavrixiaDb"];
            var secretValue3 = _configuration["Testy"];
            var secretValu4 = _configuration["ConnectionStrings:TavrixiaDb"];
            var secretValue5 = _configuration.GetConnectionString("TavrixiaDb");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
