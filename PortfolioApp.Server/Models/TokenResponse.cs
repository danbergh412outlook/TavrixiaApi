using System.Text.Json.Serialization;

namespace PortfolioApp.Server.Models
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        public DateTime ExpiresInDate
        {
            get
            {
                return DateTime.UtcNow.AddSeconds(ExpiresIn - 60);
            }
        }
    }
}
