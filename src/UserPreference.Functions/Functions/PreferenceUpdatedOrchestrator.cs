using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using UserPreference.Functions.Models;

namespace UserPreference.Functions.Functions;

public class PreferenceUpdatedOrchestrator
{
    [Function("PreferenceUpdatedOrchestrator")]
    public async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var preferences = context.GetInput<UserPreferences>();
        
        // Log the preference change
        await context.CallActivityAsync("LogPreferenceChange", preferences);
        
        // Notify external systems
        await context.CallActivityAsync("NotifyExternalSystem", preferences);
        
        // Update analytics if enabled
        if (preferences.AnalyticsEnabled)
        {
            await context.CallActivityAsync("UpdateAnalytics", preferences);
        }
        
        // Send notifications if enabled
        if (preferences.NotificationsEnabled)
        {
            await context.CallActivityAsync("SendUserNotification", preferences);
        }
    }
}

public class LogPreferenceChange
{
    [Function("LogPreferenceChange")]
    public void Run([ActivityTrigger] UserPreferences preferences, ILogger<LogPreferenceChange> logger)
    {
        logger.LogInformation("User {UserId} updated preferences: Theme={Theme}, Language={Language}, Timezone={Timezone}", 
            preferences.UserId, preferences.Theme, preferences.Language, preferences.Timezone);
    }
}

public class NotifyExternalSystem
{
    [Function("NotifyExternalSystem")]
    public async Task Run([ActivityTrigger] UserPreferences preferences, ILogger<NotifyExternalSystem> logger)
    {
        logger.LogInformation("Notifying external systems about preference update for user {UserId}", preferences.UserId);
        
        // Simulate external system notification
        await Task.Delay(1000);
        
        logger.LogInformation("External systems notified successfully for user {UserId}", preferences.UserId);
    }
}

public class UpdateAnalytics
{
    [Function("UpdateAnalytics")]
    public async Task Run([ActivityTrigger] UserPreferences preferences, ILogger<UpdateAnalytics> logger)
    {
        logger.LogInformation("Updating analytics for user {UserId}", preferences.UserId);
        
        // Simulate analytics update
        await Task.Delay(500);
        
        logger.LogInformation("Analytics updated successfully for user {UserId}", preferences.UserId);
    }
}

public class SendUserNotification
{
    [Function("SendUserNotification")]
    public async Task Run([ActivityTrigger] UserPreferences preferences, ILogger<SendUserNotification> logger)
    {
        logger.LogInformation("Sending notification to user {UserId} about preference update", preferences.UserId);
        
        // Simulate notification sending
        await Task.Delay(800);
        
        logger.LogInformation("Notification sent successfully to user {UserId}", preferences.UserId);
    }
}
