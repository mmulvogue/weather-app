namespace MM.WeatherService.Api.OpenWeatherMapApi.Models;

public class WeatherCondition
{
    public int Id { get; set; }
    public string Main { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}