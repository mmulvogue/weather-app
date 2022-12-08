using Microsoft.Extensions.Options;
using MM.WeatherService.Api.OpenWeatherMapApi.Models;
using RestSharp;

namespace MM.WeatherService.Api.OpenWeatherMapApi
{
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
            _config = options.Value;

            var clientOptions = new RestClientOptions(_config.ApiBaseUrl);
            _client = new RestClient(clientOptions);
        }

        public async Task<CurrentWeather?> GetCurrentWeatherForCityAsync(string cityName, string countryCode)
        {
            var response = await _client.GetJsonAsync<CurrentWeather>("data/2.5/weather", new
            {
                q = $"{cityName},{countryCode}",
                appid = _config.ApiKey
            });

            _logger.LogDebug("GetCurrentWeatherForCity response: {@CurrentWeatherResponse}", response);
            
            return response;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}