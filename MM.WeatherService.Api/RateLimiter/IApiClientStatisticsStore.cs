namespace MM.WeatherService.Api.RateLimiter
{
    public interface IApiClientStatisticsStore
    {
        Task<ApiClientStatistics?> GetAsync(string clientId);
        Task SetAsync(string clientId, ApiClientStatistics clientStatistics, TimeSpan expiry);
    }
}
