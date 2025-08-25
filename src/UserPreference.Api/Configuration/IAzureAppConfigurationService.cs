namespace UserPreference.Api.Configuration;

public interface IAzureAppConfigurationService
{
    Task<string?> GetConfigurationValueAsync(string key);
    Task<Dictionary<string, string>> GetConfigurationValuesAsync(string keyPrefix);
    Task SetConfigurationValueAsync(string key, string value);
}
