using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Text.Json;

namespace MM.WeatherService.Api.OpenWeatherMapApi
{
    public interface IOpenWeatherMapApiClient
    {
        Task<CurrentWeather?> GetCurrentWeatherForCityAsync(string cityName, string countryCode);
    }

    public class OpenWeatherMapApiClient : IOpenWeatherMapApiClient, IDisposable
    {
        private readonly ILogger<OpenWeatherMapApiClient> _logger;
        private readonly RestClient _client;
        private readonly OpenWeatherMapApiClientOptions _config;

        public OpenWeatherMapApiClient(
            IOptions<OpenWeatherMapApiClientOptions> options,
            ILogger<OpenWeatherMapApiClient> logger
        )
        {
            _logger = logger;

            var clientOptions = new RestClientOptions("https://api.openweathermap.org/data/2.5");
            _client = new RestClient(clientOptions);

            _config = options.Value;
            _logger.LogInformation($"OpenWeatherMapApiClient initialised. ApiKey: {_config.ApiKey}");
        }

        public async Task<CurrentWeather?> GetCurrentWeatherForCityAsync(string cityName, string countryCode)
        {
            var response = await _client.GetJsonAsync<CurrentWeather>("weather", new
            {
                q = $"{cityName},{countryCode}",
                appid = _config.ApiKey
            });

            _logger.LogDebug("GetCurrentWeatherForCity response: {@Response}", response);
            
            return response;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }

    public class OpenWeatherMapApiClientOptions
    {
        internal const string SectionName = "OpenWeatherMapApiClient";

        public string? ApiKey { get; set; }
    }
}