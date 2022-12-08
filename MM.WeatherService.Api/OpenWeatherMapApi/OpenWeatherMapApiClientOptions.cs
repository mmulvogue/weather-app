namespace MM.WeatherService.Api.OpenWeatherMapApi;

public class OpenWeatherMapApiClientOptions
{
    internal const string SectionName = "OpenWeatherMapApiClient";

    public string? ApiBaseUrl { get; set; }
    public string? ApiKey { get; set; }
}
