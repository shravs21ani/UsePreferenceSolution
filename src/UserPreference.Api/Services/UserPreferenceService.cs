using UserPreference.Api.Models;
using UserPreference.Api.Data;

namespace UserPreference.Api.Services;

public class UserPreferenceService : IUserPreferenceService
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IServiceBusService _serviceBusService;
    private readonly ILogger<UserPreferenceService> _logger;

    public UserPreferenceService(
        ICosmosDbService cosmosDbService,
        IServiceBusService serviceBusService,
        ILogger<UserPreferenceService> logger)
    {
        _cosmosDbService = cosmosDbService;
        _serviceBusService = serviceBusService;
        _logger = logger;
    }

    public async Task<UserPreferences?> GetPreferencesAsync(string userId)
    {
        try
        {
            var preferences = await _cosmosDbService.GetItemAsync<UserPreferences>(userId);
            return preferences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task<UserPreferences> CreatePreferencesAsync(string userId, CreateUserPreferencesRequest? request = null)
    {
        try
        {
            var preferences = new UserPreferences
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Theme = request?.Theme ?? "light",
                Language = request?.Language ?? "en",
                Timezone = request?.Timezone ?? "UTC",
                NotificationsEnabled = request?.NotificationsEnabled ?? true,
                AnalyticsEnabled = request?.AnalyticsEnabled ?? true,
                CustomSettings = request?.CustomSettings ?? new Dictionary<string, object>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdPreferences = await _cosmosDbService.CreateItemAsync(preferences);
            
            // Publish event to Service Bus
            await _serviceBusService.PublishMessageAsync("PreferenceCreated", createdPreferences);
            
            _logger.LogInformation("Created preferences for user {UserId}", userId);
            return createdPreferences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task<UserPreferences?> UpdatePreferencesAsync(string userId, UpdateUserPreferencesRequest request)
    {
        try
        {
            var existingPreferences = await _cosmosDbService.GetItemAsync<UserPreferences>(userId);
            if (existingPreferences == null)
            {
                return null;
            }

            // Update only the properties that are provided
            if (request.Theme != null)
                existingPreferences.Theme = request.Theme;
            
            if (request.Language != null)
                existingPreferences.Language = request.Language;
            
            if (request.Timezone != null)
                existingPreferences.Timezone = request.Timezone;
            
            if (request.NotificationsEnabled.HasValue)
                existingPreferences.NotificationsEnabled = request.NotificationsEnabled.Value;
            
            if (request.AnalyticsEnabled.HasValue)
                existingPreferences.AnalyticsEnabled = request.AnalyticsEnabled.Value;
            
            if (request.CustomSettings != null)
                existingPreferences.CustomSettings = request.CustomSettings;

            existingPreferences.UpdatedAt = DateTime.UtcNow;

            var updatedPreferences = await _cosmosDbService.UpdateItemAsync(existingPreferences);
            
            // Publish event to Service Bus
            await _serviceBusService.PublishMessageAsync("PreferenceUpdated", updatedPreferences);
            
            _logger.LogInformation("Updated preferences for user {UserId}", userId);
            return updatedPreferences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> DeletePreferencesAsync(string userId)
    {
        try
        {
            var success = await _cosmosDbService.DeleteItemAsync<UserPreferences>(userId);
            
            if (success)
            {
                // Publish event to Service Bus
                await _serviceBusService.PublishMessageAsync("PreferenceDeleted", new { UserId = userId });
                _logger.LogInformation("Deleted preferences for user {UserId}", userId);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preferences for user {UserId}", userId);
            throw;
        }
    }
}
