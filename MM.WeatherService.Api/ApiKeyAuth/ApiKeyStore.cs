namespace MM.WeatherService.Api.ApiKeyAuth;

public interface IApiKeyStore
{
    Task<bool> ExistsAsync(string apiKey);
}

public class HardCodedApiKeyStore : IApiKeyStore
{
    // This would most likely be implemented as a separate data store class backed by a db
    // and potentially a distributed cache if needed.
    private readonly string[] _apiKeyStore =
    {
        "API-dummy.key.1",
        "API-dummy.key.2",
        "API-dummy.key.3",
        "API-dummy.key.4",
        "API-dummy.key.5"
    };

    public Task<bool> ExistsAsync(string apiKey)
    {
        return Task.FromResult(_apiKeyStore.Contains(apiKey));
    }
}
