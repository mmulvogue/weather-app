namespace MM.WeatherService.Api.Models;

/// <summary>
/// Over the wire model for returning weather info
/// </summary>
public class WeatherDto
{
    /// <summary>
    /// Human readable description of the weather
    /// </summary>
    public string Description { get; set; }
}