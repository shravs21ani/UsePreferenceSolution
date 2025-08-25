using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserPreference.Api.Models;

public class UserPreferences
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("userId")]
    [Required]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("theme")]
    public string Theme { get; set; } = "light";

    [JsonPropertyName("language")]
    public string Language { get; set; } = "en";

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = "UTC";

    [JsonPropertyName("notificationsEnabled")]
    public bool NotificationsEnabled { get; set; } = true;

    [JsonPropertyName("analyticsEnabled")]
    public bool AnalyticsEnabled { get; set; } = true;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("customSettings")]
    public Dictionary<string, object> CustomSettings { get; set; } = new();

    [JsonPropertyName("etag")]
    public string ETag { get; set; } = string.Empty;
}

public class CreateUserPreferencesRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public string Theme { get; set; } = "light";
    public string Language { get; set; } = "en";
    public string Timezone { get; set; } = "UTC";
    public bool NotificationsEnabled { get; set; } = true;
    public bool AnalyticsEnabled { get; set; } = true;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}

public class UpdateUserPreferencesRequest
{
    public string? Theme { get; set; }
    public string? Language { get; set; }
    public string? Timezone { get; set; }
    public bool? NotificationsEnabled { get; set; }
    public bool? AnalyticsEnabled { get; set; }
    public Dictionary<string, object>? CustomSettings { get; set; }
}
