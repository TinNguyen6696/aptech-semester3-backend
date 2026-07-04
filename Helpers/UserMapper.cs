using TalentShowcase.Api.DTOs.Auth;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Helpers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user) => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            FirstName = user.Profile!.FirstName,
            LastName = user.Profile.LastName,
            Bio = user.Profile.Bio,
            PhoneNumber = user.Profile.PhoneNumber,
            ProfileImageUrl = user.Profile.ProfileImageUrl,
            PrimaryCategory = user.Profile.PrimaryCategory,
            SkillLevel = user.Profile.SkillLevel,
            ProvinceId = user.Profile.ProvinceId,
            ProvinceName = user.Profile.Province!.Name
        };
    }
}
