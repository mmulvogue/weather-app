using System.Security.Claims;
using Microsoft.Extensions.Options;
using MM.WeatherService.Api.RateLimiter.ApiClientStatistics;
using MM.WeatherService.Api.RateLimiter.AsyncLock;

namespace MM.WeatherService.Api.RateLimiter;

public class RateLimiterMiddleware
{
    private static readonly AsyncKeyedLock AsyncLock = new();
    private readonly IApiClientStatisticsStore _clientStatisticsStore;
    private readonly ILogger<RateLimiterMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly RateLimiterOptions _options;

    public RateLimiterMiddleware(
        RequestDelegate next,
        IApiClientStatisticsStore clientStatisticsStore,
        IOptions<RateLimiterOptions> options,
        ILogger<RateLimiterMiddleware> logger)
    {
        _next = next;
        _clientStatisticsStore = clientStatisticsStore;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Identify client using api key
        var clientId = GetClientId(context);
        if (clientId != null)
        {
            // Concurrent threads trying to run the below for the the same clientId
            // need to wait until lock is released for that clientId
            var stats = await GetUpdatedStatistics(clientId);

            if (stats.RequestCounterStartTime + TimeSpan.FromMinutes(_options.RequestLimitWindowMinutes) >
                DateTime.UtcNow)
            {
                // If client has used more than allowed requests in time window
                if (stats.RequestCounter > _options.RequestLimit)
                {
                    _logger.LogWarning($"Request rate limit reached by client: {clientId}");
                    await ReturnTooManyRequestsResponse(context,
                        stats.RequestCounterStartTime + TimeSpan.FromMinutes(_options.RequestLimitWindowMinutes));
                    return;
                }
            }
        }
        else
        {
            _logger.LogWarning("Received request with no client identity, unable to apply rate limiting");
        }

        await _next.Invoke(context);
    }

    private Task ReturnTooManyRequestsResponse(HttpContext httpContext, DateTime retryAfter)
    {
        httpContext.Response.Headers["Retry-After"] = retryAfter.ToUniversalTime().ToString("r");
        httpContext.Response.StatusCode = 429;
        httpContext.Response.ContentType = "text/plain";
        return httpContext.Response.WriteAsync(
            "API request rate limit reached. Maximum api call rate is 5 requests per hour.");
    }

    private async Task<ApiClientStatistics.ApiClientStatistics> GetUpdatedStatistics(string clientId)
    {
        var stats = new ApiClientStatistics.ApiClientStatistics
        {
            RequestCounter = 1,
            RequestCounterStartTime = DateTime.UtcNow
        };

        // Use a lock / thread synchronization to limit thread executing
        // below code per clientId to ensure statistics are accurate
        using (await AsyncLock.LockAsync(clientId))
        {
            // Update client stats
            var storedStats = await _clientStatisticsStore.GetAsync(clientId);
            if (storedStats != null)
                if (storedStats.RequestCounterStartTime + TimeSpan.FromMinutes(_options.RequestLimitWindowMinutes) >=
                    DateTime.UtcNow)
                {
                    storedStats.RequestCounter++;
                    stats = storedStats;
                }

            await _clientStatisticsStore.SetAsync(clientId, stats,
                TimeSpan.FromMinutes(_options.RequestLimitWindowMinutes));
        }

        return stats;
    }

    private string? GetClientId(HttpContext context)
    {
        // Name identifier claim is set to api key
        var identityClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return identityClaim?.Value;
    }
}
