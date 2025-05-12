using System.Text.Json.Serialization;

namespace PortfolioApp.Server.Models
{
    public class WeatherForecast
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("temperatureC")]
        public int TemperatureC { get; set; }

        [JsonPropertyName("temperatureF")]
        public int TemperatureF { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }
    }
}
