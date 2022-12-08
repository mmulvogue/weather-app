using Microsoft.AspNetCore.Authentication;

namespace MM.WeatherService.Api.ApiKeyAuth
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string SchemeName = "ApiKey";
        public string HeaderName { get; set; } = "x-api-key";
        public string FailureMessage  { get; set; } = "Invalid api key";
    }
}
