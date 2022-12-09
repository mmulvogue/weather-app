using Microsoft.AspNetCore.Authentication;

namespace MM.WeatherService.Api.ApiKeyAuth;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "ApiKey";

    /// <summary>
    ///     Gets or sets the header to use for api key authentication
    /// </summary>
    public string HeaderName { get; set; } = "x-api-key";

    /// <summary>
    ///     Gets or set the message to use api key authentication fails
    /// </summary>
    public string FailureMessage { get; set; } = "Invalid api key";
}
