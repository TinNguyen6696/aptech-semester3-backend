using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Communities
{
    public class CommunityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public TalentCategory Category { get; set; }
        public int PostCount { get; set; }
    }
}