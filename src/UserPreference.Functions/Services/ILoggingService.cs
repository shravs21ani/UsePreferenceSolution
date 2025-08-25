namespace UserPreference.Functions.Services;

public interface ILoggingService
{
    Task LogPreferenceChangeAsync(string userId, string changeType, object changeData);
    Task LogSystemEventAsync(string eventType, string message, object? data = null);
}
