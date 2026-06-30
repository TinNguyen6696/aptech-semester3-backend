namespace TalentShowcase.Api.DTOs
{
    public class OptionsDto
    {
        public IEnumerable<string> Roles { get; set; } = null!;
        public IEnumerable<string> TalentCategories { get; set; } = null!;
        public IEnumerable<string> SkillLevels { get; set; } = null!;
    }
}
