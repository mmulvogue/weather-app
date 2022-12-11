namespace MM.WeatherService.Api.RateLimiter.ApiClientStatistics;

public interface IApiClientStatisticsStore
{
    Task<ApiClientStatistics?> GetAsync(string clientId);
    Task SetAsync(string clientId, ApiClientStatistics clientStatistics, TimeSpan expiry);
}
