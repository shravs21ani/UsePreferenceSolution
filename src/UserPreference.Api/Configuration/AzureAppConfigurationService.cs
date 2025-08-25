using Azure.Data.AppConfiguration;

namespace UserPreference.Api.Configuration;

public class AzureAppConfigurationService : IAzureAppConfigurationService
{
    private readonly ConfigurationClient _client;
    private readonly ILogger<AzureAppConfigurationService> _logger;

    public AzureAppConfigurationService(string connectionString)
    {
        _client = new ConfigurationClient(connectionString);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AzureAppConfigurationService>();
    }

    public async Task<string?> GetConfigurationValueAsync(string key)
    {
        try
        {
            var response = await _client.GetConfigurationSettingAsync(key);
            return response.Value?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configuration value for key {Key}", key);
            return null;
        }
    }

    public async Task<Dictionary<string, string>> GetConfigurationValuesAsync(string keyPrefix)
    {
        try
        {
            var settings = new Dictionary<string, string>();
            var selector = new SettingSelector { KeyFilter = $"{keyPrefix}*" };
            
            await foreach (var setting in _client.GetConfigurationSettingsAsync(selector))
            {
                if (setting.Key != null && setting.Value != null)
                {
                    settings[setting.Key] = setting.Value;
                }
            }
            
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configuration values for prefix {KeyPrefix}", keyPrefix);
            return new Dictionary<string, string>();
        }
    }

    public async Task SetConfigurationValueAsync(string key, string value)
    {
        try
        {
            var setting = new ConfigurationSetting(key, value);
            await _client.SetConfigurationSettingAsync(setting);
            
            _logger.LogInformation("Set configuration value for key {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting configuration value for key {Key}", key);
            throw;
        }
    }
}
