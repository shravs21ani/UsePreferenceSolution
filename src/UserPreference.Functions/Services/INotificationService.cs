namespace UserPreference.Functions.Services;

public interface INotificationService
{
    Task SendNotificationAsync(string userId, string message, string notificationType);
    Task SendEmailAsync(string email, string subject, string body);
}
