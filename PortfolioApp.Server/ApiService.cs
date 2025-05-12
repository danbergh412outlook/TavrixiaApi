using PortfolioApp.Server.Models;
using RestSharp;
using System.Text.Json;

namespace PortfolioApp.Server
{
    public class ApiService
    {
        private readonly IConfiguration _configuration;
        private TokenResponse _cachedToken;

        public ApiService(IConfiguration configuration)
        {
            _configuration = configuration; // Access app settings
        }

        public TokenResponse GetToken()
        {
            var tokenUrl = _configuration["ApiCalls:TokenUrl"];
            var clientId = _configuration["ApiCalls:ClientId"];
            var clientSecret = _configuration["ApiCalls:ClientSecret"];
            var scope = _configuration["ApiCalls:Scope"];

            try
            {
                // Check if the cached token is still valid
                if (_cachedToken != null && DateTime.UtcNow < _cachedToken.ExpiresInDate)
                {
                    return _cachedToken;
                }
                

                // Token has expired or does not exist; fetch a new one
                var options = new RestClientOptions(tokenUrl);
                var client = new RestClient(options);
                var request = new RestRequest()
                    .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                    .AddParameter("grant_type", "client_credentials")
                    .AddParameter("client_id", clientId)
                    .AddParameter("client_secret", clientSecret)
                    .AddParameter("scope", scope);

                var response = client.Post(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    _cachedToken = JsonSerializer.Deserialize<TokenResponse>(response.Content);
                }

                return _cachedToken;
            }
            catch(Exception ex)
            {
                throw new Exception($"'{tokenUrl}' '{clientId}' '{clientSecret}' '{scope}'", ex);
            }
            
        }

        public T GetRequest<T>(string route)
        {
            // Get the access token
            var tokenResponse = GetToken();

            // Call the API
            var apiOptions = new RestClientOptions(_configuration["ApiCalls:BaseUrl"] + "/" + route);
            var apiClient = new RestClient(apiOptions);
            var apiRequest = new RestRequest()
                .AddHeader("Authorization", $"Bearer {tokenResponse.AccessToken}");

            var apiResponse = apiClient.Get(apiRequest);

            // Deserialize and return the data
            var returnData = JsonSerializer.Deserialize<T>(apiResponse.Content);
            return returnData;
        }
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            return GetRequest<IEnumerable<WeatherForecast>>("WeatherForecast");
        }
    }
}
