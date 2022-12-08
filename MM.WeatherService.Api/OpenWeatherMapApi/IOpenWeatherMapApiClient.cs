using MM.WeatherService.Api.OpenWeatherMapApi.Models;

namespace MM.WeatherService.Api.OpenWeatherMapApi;

public interface IOpenWeatherMapApiClient
{
    Task<CurrentWeather?> GetCurrentWeatherForCityAsync(string cityName, string countryCode);
}