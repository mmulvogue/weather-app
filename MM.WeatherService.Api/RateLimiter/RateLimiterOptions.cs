namespace MM.WeatherService.Api.RateLimiter;

public class RateLimiterOptions
{
    internal const string SectionName = "RateLimiter";

    public int RequestLimit { get; set; } = 5;
    public int RequestLimitWindowMinutes { get; set; } = 60;
}
