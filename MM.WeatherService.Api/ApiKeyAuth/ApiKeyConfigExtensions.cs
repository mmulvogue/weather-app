using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MM.WeatherService.Api.ApiKeyAuth;

public static class ApiKeyConfigExtensions
{
    /// <summary>
    ///     Set up the services required for api key authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="apiKeyHeaderName">name of HTTP header to be used for api key</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddApiKeyAuthentication(this IServiceCollection services,
        string apiKeyHeaderName)
    {
        services.AddSingleton<IApiKeyStore, HardCodedApiKeyStore>();
        services.AddTransient<IApiKeyService, ApiKeyService>();
        return services.AddAuthentication(options => { options.DefaultScheme = ApiKeyAuthenticationOptions.SchemeName; }
        ).AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.SchemeName,
            options => { options.HeaderName = apiKeyHeaderName; });
    }

    /// <summary>
    ///     Sets up security config in Swagger to support calling api with api key
    /// </summary>
    /// <param name="options"></param>
    /// <param name="apiKeyHeaderName">name of HTTP header to be used for api key</param>
    public static void AddApiKeySecurityRequirement(this SwaggerGenOptions options, string apiKeyHeaderName)
    {
        // Add api key security scheme
        options.AddSecurityDefinition(ApiKeyAuthenticationOptions.SchemeName, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = apiKeyHeaderName,
            Type = SecuritySchemeType.ApiKey
        });

        // Require API key auth for all endpoints
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyAuthenticationOptions.SchemeName
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}
