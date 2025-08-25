using Microsoft.Extensions.Logging;

namespace UserPreference.Functions.Services;

public class LoggingService : ILoggingService
{
    private readonly ILogger<LoggingService> _logger;

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
    }

    public async Task LogPreferenceChangeAsync(string userId, string changeType, object changeData)
    {
        try
        {
            _logger.LogInformation("Preference change logged: User={UserId}, Type={ChangeType}, Data={@ChangeData}", 
                userId, changeType, changeData);
            
            // In a real implementation, you might log to a database or external logging service
            await Task.Delay(100); // Simulate async operation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging preference change for user {UserId}", userId);
        }
    }

    public async Task LogSystemEventAsync(string eventType, string message, object? data = null)
    {
        try
        {
            _logger.LogInformation("System event logged: Type={EventType}, Message={Message}, Data={@Data}", 
                eventType, message, data);
            
            // In a real implementation, you might log to a database or external logging service
            await Task.Delay(100); // Simulate async operation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging system event: {EventType}", eventType);
        }
    }
}
