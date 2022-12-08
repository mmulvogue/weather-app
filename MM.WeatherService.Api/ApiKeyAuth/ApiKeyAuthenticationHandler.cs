using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MM.WeatherService.Api.ApiKeyAuth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly ILogger<ApiKeyAuthenticationHandler> _logger;
        private readonly IApiKeyService _apiKeyService;
        private readonly ApiKeyAuthenticationOptions _options;

        public ApiKeyAuthenticationHandler(
            ILogger<ApiKeyAuthenticationHandler> logger,
            IOptionsMonitor<ApiKeyAuthenticationOptions> optionsMonitor,
            IApiKeyService apiKeyService,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(optionsMonitor, loggerFactory, encoder, clock)
        {
            _logger = logger;
            _apiKeyService = apiKeyService;
            _options = optionsMonitor.CurrentValue;
            _logger.LogInformation("ApiKeyAuthenticationHandler constructed");
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Validate that api key header has been provided
            _logger.LogInformation($"Verifying api key header: {_options.HeaderName}");
            if (!Request.Headers.TryGetValue(_options.HeaderName, out var apiKeyHeaderValues))
            {
                _logger.LogWarning("An API request was received without the x-api-key header");
                return AuthenticateResult.Fail(_options.FailureMessage);
            }
            if (apiKeyHeaderValues.Count != 1)
            {
                _logger.LogWarning("More than one x-api-key header was sent");
                return AuthenticateResult.Fail(_options.FailureMessage);
            }

            // Validate that provided string in header is a valid api key
            var apiKey = apiKeyHeaderValues.First();
            _logger.LogInformation($"Received api key: {apiKey}");
            if (!await _apiKeyService.IsValidKeyAsync(apiKey))
            {
                return AuthenticateResult.Fail(_options.FailureMessage);
            }

            // Using a dummy key owner for simplicity
            // Would most likely lookup key owner in a user store here
            // and setup ClaimsPrincipal from that
            var apiKeyOwner = new
            {
                Name = $"DummyKeyOwner of {apiKey}"
            };
            
            var claims = new[] { new Claim(ClaimTypes.Name, apiKeyOwner.Name) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);

        }
    }
}
