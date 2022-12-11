using Microsoft.Extensions.Caching.Memory;

namespace MM.WeatherService.Api.RateLimiter.ApiClientStatistics;

public class InMemoryApiClientStatisticsStore : IApiClientStatisticsStore
{
    private const string CacheKeyPrefix = "ApiClientStatistics";
    private readonly IMemoryCache _cache;

    public InMemoryApiClientStatisticsStore(
        IMemoryCache cache
    )
    {
        _cache = cache;
    }

    public Task<ApiClientStatistics?> GetAsync(string clientId)
    {
        return Task.FromResult(_cache.Get<ApiClientStatistics?>($"{CacheKeyPrefix}:{clientId}"));
    }

    public Task SetAsync(string clientId, ApiClientStatistics clientStatistics, TimeSpan expiry)
    {
        _cache.Set($"{CacheKeyPrefix}:{clientId}", clientStatistics, expiry);
        return Task.CompletedTask;
    }
}
