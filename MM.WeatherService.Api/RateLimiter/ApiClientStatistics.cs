namespace MM.WeatherService.Api.RateLimiter
{
    public class ApiClientStatistics
    {
        public DateTime RequestCounterStartTime { get; set; }
        public int RequestCounter { get; set; }
    }
}
