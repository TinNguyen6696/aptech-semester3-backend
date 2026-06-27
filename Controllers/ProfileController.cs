using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaLentShowcase.API.DTOS.Profile;
using TaLentShowcase.API.Middleware;
using TaLentShowcase.API.Models;
using TaLentShowcase.API.Services.Interfaces;

namespace TaLentShowcase.API.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [AllowAnonymous]
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetProfile(int userId)
    {
        var result = await _profileService.GetProfileAsync(userId);
        return Ok(ApiResponse<ProfileResponse>.Ok(result, "Profile retrieved successfully."));
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> UpdateProfile(int userId, UpdateProfileRequest request)
    {
        EnsureCanModify(userId);
        await _profileService.UpdateProfileAsync(userId, request);
        return Ok(ApiResponse<object?>.Ok(null, "Profile updated successfully."));
    }

    [HttpPost("{userId:int}/talents")]
    public async Task<IActionResult> AddUserTalent(int userId, AddUserTalentRequest request)
    {
        EnsureCanModify(userId);
        var result = await _profileService.AddUserTalentAsync(userId, request);
        return CreatedAtAction(
            nameof(GetProfile),
            new { userId },
            ApiResponse<UserTalentDto>.Ok(result, "Talent added successfully."));
    }

    [HttpPut("{userId:int}/talents/{userTalentId:int}")]
    public async Task<IActionResult> UpdateUserTalent(
        int userId,
        int userTalentId,
        UpdateUserTalentRequest request)
    {
        EnsureCanModify(userId);
        await _profileService.UpdateUserTalentAsync(userId, userTalentId, request);
        return Ok(ApiResponse<object?>.Ok(null, "Talent updated successfully."));
    }

    [HttpDelete("{userId:int}/talents/{userTalentId:int}")]
    public async Task<IActionResult> DeleteUserTalent(int userId, int userTalentId)
    {
        EnsureCanModify(userId);
        await _profileService.DeleteUserTalentAsync(userId, userTalentId);
        return Ok(ApiResponse<object?>.Ok(null, "Talent deleted successfully."));
    }

    [HttpPost("{userId:int}/achievements")]
    public async Task<IActionResult> AddAchievement(int userId, UpsertAchievementRequest request)
    {
        EnsureCanModify(userId);
        var result = await _profileService.AddAchievementAsync(userId, request);
        return CreatedAtAction(
            nameof(GetProfile),
            new { userId },
            ApiResponse<AchievementDto>.Ok(result, "Achievement added successfully."));
    }

    [HttpPut("{userId:int}/achievements/{achievementId:int}")]
    public async Task<IActionResult> UpdateAchievement(
        int userId,
        int achievementId,
        UpsertAchievementRequest request)
    {
        EnsureCanModify(userId);
        await _profileService.UpdateAchievementAsync(userId, achievementId, request);
        return Ok(ApiResponse<object?>.Ok(null, "Achievement updated successfully."));
    }

    [HttpDelete("{userId:int}/achievements/{achievementId:int}")]
    public async Task<IActionResult> DeleteAchievement(int userId, int achievementId)
    {
        EnsureCanModify(userId);
        await _profileService.DeleteAchievementAsync(userId, achievementId);
        return Ok(ApiResponse<object?>.Ok(null, "Achievement deleted successfully."));
    }

    [HttpPost("{userId:int}/awards")]
    public async Task<IActionResult> AddAward(int userId, UpsertAwardRequest request)
    {
        EnsureCanModify(userId);
        var result = await _profileService.AddAwardAsync(userId, request);
        return CreatedAtAction(
            nameof(GetProfile),
            new { userId },
            ApiResponse<AwardDto>.Ok(result, "Award added successfully."));
    }

    [HttpPut("{userId:int}/awards/{awardId:int}")]
    public async Task<IActionResult> UpdateAward(int userId, int awardId, UpsertAwardRequest request)
    {
        EnsureCanModify(userId);
        await _profileService.UpdateAwardAsync(userId, awardId, request);
        return Ok(ApiResponse<object?>.Ok(null, "Award updated successfully."));
    }

    [HttpDelete("{userId:int}/awards/{awardId:int}")]
    public async Task<IActionResult> DeleteAward(int userId, int awardId)
    {
        EnsureCanModify(userId);
        await _profileService.DeleteAwardAsync(userId, awardId);
        return Ok(ApiResponse<object?>.Ok(null, "Award deleted successfully."));
    }

    [HttpPost("{userId:int}/certifications")]
    public async Task<IActionResult> AddCertification(
        int userId,
        UpsertCertificationRequest request)
    {
        EnsureCanModify(userId);
        var result = await _profileService.AddCertificationAsync(userId, request);
        return CreatedAtAction(
            nameof(GetProfile),
            new { userId },
            ApiResponse<CertificationDto>.Ok(result, "Certification added successfully."));
    }

    [HttpPut("{userId:int}/certifications/{certificationId:int}")]
    public async Task<IActionResult> UpdateCertification(
        int userId,
        int certificationId,
        UpsertCertificationRequest request)
    {
        EnsureCanModify(userId);
        await _profileService.UpdateCertificationAsync(userId, certificationId, request);
        return Ok(ApiResponse<object?>.Ok(null, "Certification updated successfully."));
    }

    [HttpDelete("{userId:int}/certifications/{certificationId:int}")]
    public async Task<IActionResult> DeleteCertification(int userId, int certificationId)
    {
        EnsureCanModify(userId);
        await _profileService.DeleteCertificationAsync(userId, certificationId);
        return Ok(ApiResponse<object?>.Ok(null, "Certification deleted successfully."));
    }

    private void EnsureCanModify(int userId)
    {
        if (User.IsInRole("Admin"))
        {
            return;
        }

        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!int.TryParse(claim, out var authenticatedUserId) || authenticatedUserId != userId)
        {
            throw new ForbiddenException("You can only modify your own profile.");
        }
    }
}
