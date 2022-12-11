using Microsoft.Extensions.Options;
using MM.WeatherService.Api.OpenWeatherMapApi.Models;
using RestSharp;

namespace MM.WeatherService.Api.OpenWeatherMapApi;

public interface IOpenWeatherMapApiClient
{
    Task<CurrentWeather?> GetCurrentWeatherForCityAsync(string cityName, string countryCode);
}

public class OpenWeatherMapApiClient : IOpenWeatherMapApiClient, IDisposable
{
    private readonly RestClient _client;
    private readonly OpenWeatherMapApiClientOptions _config;
    private readonly ILogger<OpenWeatherMapApiClient> _logger;

    public OpenWeatherMapApiClient(
        IOptions<OpenWeatherMapApiClientOptions> options,
        ILogger<OpenWeatherMapApiClient> logger
    )
    {
        _logger = logger;
        _config = options.Value;

        if (_config?.ApiBaseUrl == null)
        {
            throw new ArgumentNullException("ApiBaseUrl from OpenWeatherMapApiClientOptions cannot be null");
        }

        var clientOptions = new RestClientOptions(_config.ApiBaseUrl);
        _client = new RestClient(clientOptions);
    }

    public void Dispose()
    {
        _client.Dispose();
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
}
