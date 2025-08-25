using Microsoft.Extensions.Logging;

namespace UserPreference.Functions.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendNotificationAsync(string userId, string message, string notificationType)
    {
        try
        {
            _logger.LogInformation("Sending {NotificationType} notification to user {UserId}: {Message}", 
                notificationType, userId, message);
            
            // In a real implementation, you would integrate with a notification service
            // like Azure Notification Hubs, SendGrid, etc.
            await Task.Delay(500); // Simulate async operation
            
            _logger.LogInformation("Notification sent successfully to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Sending email to {Email}: {Subject}", email, subject);
            
            // In a real implementation, you would integrate with an email service
            // like SendGrid, Azure Communication Services, etc.
            await Task.Delay(800); // Simulate async operation
            
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email);
        }
    }
}
