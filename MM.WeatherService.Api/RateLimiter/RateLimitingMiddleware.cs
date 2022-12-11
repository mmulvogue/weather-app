using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Security.Principal;

namespace MM.WeatherService.Api.RateLimiter
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApiClientStatisticsStore _clientStatisticsStore;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly AsyncKeyedLock AsyncLock = new AsyncKeyedLock();

        public RateLimitingMiddleware(
            RequestDelegate next,
            IApiClientStatisticsStore clientStatisticsStore,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _clientStatisticsStore = clientStatisticsStore;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Identify client using api key
            var clientId = GetClientId(context);

            // Concurrent threads trying to run the below for the the same clientId
            // need to wait until lock is released for that clientId
            var stats = await GetUpdatedStatistics(clientId);

            if (stats.RequestCounterStartTime + TimeSpan.FromMinutes(60) > DateTime.UtcNow)
            {
                // If client has used more than allowed requests in time window
                if (stats.RequestCounter > 5)
                {
                    await ReturnQuotaExceededResponse(context);
                    return;
                }
            }
        }

        private Task ReturnQuotaExceededResponse(HttpContext httpContext)
        {
            //httpContext.Response.Headers["Retry-After"] = retryAfter;
            httpContext.Response.StatusCode = 429;
            httpContext.Response.ContentType = "text/plain";
            return httpContext.Response.WriteAsync("API calls quota exceeded! maximum admitted 5 per hour.");
        }

        private async Task<ApiClientStatistics> GetUpdatedStatistics(string clientId)
        {
            var trackingWindow = TimeSpan.FromSeconds(60);
            var stats = new ApiClientStatistics
            {
                RequestCounter = 1,
                RequestCounterStartTime = DateTime.UtcNow
            };

            using (await AsyncLock.LockAsync(clientId, 500))
            {
                // Update client stats
                var storedStats = await _clientStatisticsStore.GetAsync(clientId);
                if (storedStats != null)
                {
                    if (storedStats.RequestCounterStartTime + trackingWindow >= DateTime.UtcNow)
                    {
                        storedStats.RequestCounter++;
                        stats = storedStats;
                    }
                }

                await _clientStatisticsStore.SetAsync(clientId, stats, trackingWindow);
            }
            return stats;
        }

        private string? GetClientId(HttpContext context)
        {
            Claim identityClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return identityClaim?.Value;
        }

        private void UpdateApiClientStatistics(string clientId)
        {
        }
    }
}
