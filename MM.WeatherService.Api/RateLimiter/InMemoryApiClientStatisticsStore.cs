using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace MM.WeatherService.Api.RateLimiter
{
    public class InMemoryApiClientStatisticsStore : IApiClientStatisticsStore
    {
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "ApiClientStatistics";

        public InMemoryApiClientStatisticsStore(
            IMemoryCache cache
        )
        {
            _cache = cache;
        }

        public Task<ApiClientStatistics> GetAsync(string clientId)
        {
            return Task.FromResult(_cache.Get<ApiClientStatistics>($"{CacheKeyPrefix}:{clientId}"));
        }

        public Task SetAsync(string clientId, ApiClientStatistics clientStatistics, TimeSpan expiry)
        {
            _cache.Set($"{CacheKeyPrefix}:{clientId}", clientStatistics, expiry);
            return Task.CompletedTask;
        }
    }
}
