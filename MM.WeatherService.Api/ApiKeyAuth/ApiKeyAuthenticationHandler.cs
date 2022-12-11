using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace MM.WeatherService.Api.ApiKeyAuth;

/// <summary>
///     Custom AspNetCore authentication handler to support authenticating with an api key
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;
    private readonly IOptionsMonitor<ApiKeyAuthenticationOptions> _optionsMonitor;

    public ApiKeyAuthenticationHandler(
        ILogger<ApiKeyAuthenticationHandler> logger,
        IApiKeyService apiKeyService,
        IOptionsMonitor<ApiKeyAuthenticationOptions> optionsMonitor,
        ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock
    ) : base(optionsMonitor, loggerFactory, encoder, clock)
    {
        _logger = logger;
        _apiKeyService = apiKeyService;
        _optionsMonitor = optionsMonitor;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var options = _optionsMonitor.CurrentValue;

        // Validate that api key header has been provided
        _logger.LogInformation($"Verifying api key header: {options.HeaderName}");
        if (!Request.Headers.TryGetValue(options.HeaderName, out var apiKeyHeaderValues)
            || StringValues.IsNullOrEmpty(apiKeyHeaderValues))
        {
            _logger.LogWarning($"No api key header ({options.HeaderName}) present on request or no value");
            return AuthenticateResult.NoResult();
        }

        if (apiKeyHeaderValues.Count > 1)
        {
            _logger.LogWarning($"More than one api key header ({options.HeaderName}) was sent");
            return AuthenticateResult.Fail(options.FailureMessage);
        }

        // Validate that provided header value is a valid api key
        var apiKey = apiKeyHeaderValues.First();
        _logger.LogDebug($"Received api key: {apiKey}");
        if (!await _apiKeyService.IsValidKeyAsync(apiKey)) return AuthenticateResult.Fail(options.FailureMessage);

        // Using a dummy key owner for simplicity
        // Would most likely lookup key owner in a user store here
        var apiKeyOwner = new
        {
            Name = $"DummyKeyOwner of {apiKey}"
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey),
            new Claim(ClaimTypes.Name, apiKeyOwner.Name)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
