namespace UserPreference.Functions.Models;

public class UserPreferences
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Theme { get; set; } = "light";
    public string Language { get; set; } = "en";
    public string Timezone { get; set; } = "UTC";
    public bool NotificationsEnabled { get; set; } = true;
    public bool AnalyticsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}
