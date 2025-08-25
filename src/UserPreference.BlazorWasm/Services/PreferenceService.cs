using System.Net.Http.Json;
using System.Text.Json;
using UserPreference.BlazorWasm.Models;

namespace UserPreference.BlazorWasm.Services;

public class PreferenceService : IPreferenceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PreferenceService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PreferenceService(HttpClient httpClient, ILogger<PreferenceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<UserPreferences> GetPreferencesAsync()
    {
        try
        {
            // For development, use a mock user ID
            var userId = "dev-user-123";
            return await GetPreferencesByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get preferences");
            throw;
        }
    }

    public async Task<UserPreferences> GetPreferencesByUserIdAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/preferences/{userId}");
            
            if (response.IsSuccessStatusCode)
            {
                var preferences = await response.Content.ReadFromJsonAsync<UserPreferences>(_jsonOptions);
                return preferences ?? CreateDefaultPreferences(userId);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return CreateDefaultPreferences(userId);
            }
            
            response.EnsureSuccessStatusCode();
            throw new HttpRequestException($"Failed to get preferences: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get preferences for user {UserId}", userId);
            throw;
        }
    }

    public async Task<UserPreferences> UpdatePreferencesAsync(UserPreferences preferences)
    {
        try
        {
            preferences.UpdatedAt = DateTime.UtcNow;
            
            var response = await _httpClient.PutAsJsonAsync($"/api/preferences/{preferences.UserId}", preferences, _jsonOptions);
            response.EnsureSuccessStatusCode();
            
            var updatedPreferences = await response.Content.ReadFromJsonAsync<UserPreferences>(_jsonOptions);
            return updatedPreferences ?? preferences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update preferences for user {UserId}", preferences.UserId);
            throw;
        }
    }

    public async Task<bool> DeletePreferencesAsync(string userId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/preferences/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete preferences for user {UserId}", userId);
            throw;
        }
    }

    private static UserPreferences CreateDefaultPreferences(string userId)
    {
        return new UserPreferences
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Theme = "light",
            Language = "en",
            Timezone = "UTC",
            NotificationsEnabled = true,
            AnalyticsEnabled = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CustomSettings = new Dictionary<string, object>()
        };
    }
}
