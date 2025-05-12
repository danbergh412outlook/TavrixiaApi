using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.Services;
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
        private readonly AppDbContext _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration, AppDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [AllowAnonymous]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var survey = await _context.Surveys
                .FirstOrDefaultAsync();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = survey?.Name ?? "No Survey"
            })
            .ToArray();
        }
    }
}
