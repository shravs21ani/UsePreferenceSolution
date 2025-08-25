using UserPreference.BlazorWasm.Models;

namespace UserPreference.BlazorWasm.Services;

public interface IPreferenceService
{
    Task<UserPreferences> GetPreferencesAsync();
    Task<UserPreferences> UpdatePreferencesAsync(UserPreferences preferences);
    Task<UserPreferences> GetPreferencesByUserIdAsync(string userId);
    Task<bool> DeletePreferencesAsync(string userId);
}
