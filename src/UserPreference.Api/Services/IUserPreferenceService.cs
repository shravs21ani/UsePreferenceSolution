using UserPreference.Api.Models;

namespace UserPreference.Api.Services;

public interface IUserPreferenceService
{
    Task<UserPreferences?> GetPreferencesAsync(string userId);
    Task<UserPreferences> CreatePreferencesAsync(string userId, CreateUserPreferencesRequest? request = null);
    Task<UserPreferences?> UpdatePreferencesAsync(string userId, UpdateUserPreferencesRequest request);
    Task<bool> DeletePreferencesAsync(string userId);
}
