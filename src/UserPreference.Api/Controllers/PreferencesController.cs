using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserPreference.Api.Models;
using UserPreference.Api.Services;
using System.Security.Claims;

namespace UserPreference.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PreferencesController : ControllerBase
{
    private readonly IUserPreferenceService _preferenceService;
    private readonly ILogger<PreferencesController> _logger;

    public PreferencesController(IUserPreferenceService preferenceService, ILogger<PreferencesController> logger)
    {
        _preferenceService = preferenceService;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserPreferences>> GetPreferences(string userId)
    {
        try
        {
            // In a real application, you would validate that the authenticated user
            // has access to the requested userId
            var preferences = await _preferenceService.GetPreferencesAsync(userId);
            
            if (preferences == null)
            {
                return NotFound($"Preferences not found for user {userId}");
            }

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preferences for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving preferences");
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserPreferences>> GetMyPreferences()
    {
        try
        {
            // Get the authenticated user's ID from the JWT token
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            var preferences = await _preferenceService.GetPreferencesAsync(userId);
            
            if (preferences == null)
            {
                // Create default preferences if none exist
                preferences = await _preferenceService.CreatePreferencesAsync(userId);
            }

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preferences for current user");
            return StatusCode(500, "An error occurred while retrieving preferences");
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserPreferences>> CreatePreferences([FromBody] CreateUserPreferencesRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preferences = await _preferenceService.CreatePreferencesAsync(request.UserId, request);
            return CreatedAtAction(nameof(GetPreferences), new { userId = request.UserId }, preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating preferences for user {UserId}", request.UserId);
            return StatusCode(500, "An error occurred while creating preferences");
        }
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<UserPreferences>> UpdatePreferences(string userId, [FromBody] UpdateUserPreferencesRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preferences = await _preferenceService.UpdatePreferencesAsync(userId, request);
            
            if (preferences == null)
            {
                return NotFound($"Preferences not found for user {userId}");
            }

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for user {UserId}", userId);
            return StatusCode(500, "An error occurred while updating preferences");
        }
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserPreferences>> UpdateMyPreferences([FromBody] UpdateUserPreferencesRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preferences = await _preferenceService.UpdatePreferencesAsync(userId, request);
            
            if (preferences == null)
            {
                // Create default preferences if none exist
                preferences = await _preferenceService.CreatePreferencesAsync(userId);
            }

            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for current user");
            return StatusCode(500, "An error occurred while updating preferences");
        }
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeletePreferences(string userId)
    {
        try
        {
            var success = await _preferenceService.DeletePreferencesAsync(userId);
            
            if (!success)
            {
                return NotFound($"Preferences not found for user {userId}");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting preferences for user {UserId}", userId);
            return StatusCode(500, "An error occurred while deleting preferences");
        }
    }

    private string? GetCurrentUserId()
    {
        // Extract user ID from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value 
                         ?? User.FindFirst("userId")?.Value;
        
        return userIdClaim;
    }
}
