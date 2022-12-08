namespace MM.WeatherService.Api.ApiKeyAuth
{
    public interface IApiKeyService
    {
        Task<bool> IsValidKeyAsync(string apiKey);
    }

    public class ApiKeyService : IApiKeyService
    {
        private readonly string[] _apiKeyStore =
        {
            "API-dummy.key.1",
            "API-dummy.key.2",
            "API-dummy.key.3",
            "API-dummy.key.4",
            "API-dummy.key.5"
        };

        public Task<bool> IsValidKeyAsync(string apiKey)
        {
            var result = _apiKeyStore.Contains(apiKey);
            return Task.FromResult(result);
        }
    }
}
