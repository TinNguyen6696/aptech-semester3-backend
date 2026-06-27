using Microsoft.AspNetCore.Mvc;
using TaLentShowcase.API.DTOS.Profile;
using TaLentShowcase.API.Services.Interfaces;

namespace TaLentShowcase.API.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetProfile(int userId)
    {
        var result = await _profileService.GetProfileAsync(userId);

        return Ok(result);
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> UpdateProfile(
        int userId,
        [FromBody] UpdateProfileRequest request)
    {
        await _profileService.UpdateProfileAsync(userId, request);

        return NoContent();
    }

    [HttpPost("{userId:int}/talents")]
    public async Task<IActionResult> AddUserTalent(
        int userId,
        [FromBody] AddUserTalentRequest request)
    {
        var result = await _profileService.AddUserTalentAsync(userId, request);

        return CreatedAtAction(nameof(GetProfile), new
        {
            userId
        }, result);
    }

    [HttpPut("{userId:int}/talents/{userTalentId:int}")]
    public async Task<IActionResult> UpdateUserTalent(
        int userId,
        int userTalentId,
        [FromBody] UpdateUserTalentRequest request)
    {
        await _profileService.UpdateUserTalentAsync(userId, userTalentId, request);

        return NoContent();
    }

    [HttpDelete("{userId:int}/talents/{userTalentId:int}")]
    public async Task<IActionResult> DeleteUserTalent(int userId, int userTalentId)
    {
        await _profileService.DeleteUserTalentAsync(userId, userTalentId);

        return NoContent();
    }

    [HttpPost("{userId:int}/achievements")]
    public async Task<IActionResult> AddAchievement(
        int userId,
        [FromBody] UpsertAchievementRequest request)
    {
        var result = await _profileService.AddAchievementAsync(userId, request);

        return CreatedAtAction(nameof(GetProfile), new
        {
            userId
        }, result);
    }

    [HttpPut("{userId:int}/achievements/{achievementId:int}")]
    public async Task<IActionResult> UpdateAchievement(
        int userId,
        int achievementId,
        [FromBody] UpsertAchievementRequest request)
    {
        await _profileService.UpdateAchievementAsync(userId, achievementId, request);

        return NoContent();
    }

    [HttpDelete("{userId:int}/achievements/{achievementId:int}")]
    public async Task<IActionResult> DeleteAchievement(int userId, int achievementId)
    {
        await _profileService.DeleteAchievementAsync(userId, achievementId);

        return NoContent();
    }

    [HttpPost("{userId:int}/awards")]
    public async Task<IActionResult> AddAward(
        int userId,
        [FromBody] UpsertAwardRequest request)
    {
        var result = await _profileService.AddAwardAsync(userId, request);

        return CreatedAtAction(nameof(GetProfile), new
        {
            userId
        }, result);
    }

    [HttpPut("{userId:int}/awards/{awardId:int}")]
    public async Task<IActionResult> UpdateAward(
        int userId,
        int awardId,
        [FromBody] UpsertAwardRequest request)
    {
        await _profileService.UpdateAwardAsync(userId, awardId, request);

        return NoContent();
    }

    [HttpDelete("{userId:int}/awards/{awardId:int}")]
    public async Task<IActionResult> DeleteAward(int userId, int awardId)
    {
        await _profileService.DeleteAwardAsync(userId, awardId);

        return NoContent();
    }

    [HttpPost("{userId:int}/certifications")]
    public async Task<IActionResult> AddCertification(
        int userId,
        [FromBody] UpsertCertificationRequest request)
    {
        var result = await _profileService.AddCertificationAsync(userId, request);

        return CreatedAtAction(nameof(GetProfile), new
        {
            userId
        }, result);
    }

    [HttpPut("{userId:int}/certifications/{certificationId:int}")]
    public async Task<IActionResult> UpdateCertification(
        int userId,
        int certificationId,
        [FromBody] UpsertCertificationRequest request)
    {
        await _profileService.UpdateCertificationAsync(userId, certificationId, request);

        return NoContent();
    }

    [HttpDelete("{userId:int}/certifications/{certificationId:int}")]
    public async Task<IActionResult> DeleteCertification(int userId, int certificationId)
    {
        await _profileService.DeleteCertificationAsync(userId, certificationId);

        return NoContent();
    }
}
