namespace MM.WeatherService.Api.ApiKeyAuth;

public interface IApiKeyService
{
    Task<bool> IsValidKeyAsync(string apiKey);
}

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyStore _apiKeyStore;

    public ApiKeyService(IApiKeyStore apiKeyStore)
    {
        _apiKeyStore = apiKeyStore;
    }

    /// <summary>
    ///     Checks if provided key is a valid api key
    /// </summary>
    /// <param name="apiKey">key to validate</param>
    /// <returns>true/false indicating if api key is valid or not</returns>
    public async Task<bool> IsValidKeyAsync(string apiKey)
    {
        // Simple shape validation check to avoid a data store lookup
        if (!apiKey.StartsWith("API-")) return false;

        // Finally check if key exists in the data store
        return await _apiKeyStore.ExistsAsync(apiKey);
    }
}
