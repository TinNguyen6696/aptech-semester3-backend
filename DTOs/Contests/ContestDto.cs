using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Contests
{
    public class ContestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TalentCategory Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int EntryCount { get; set; }
        public int? WinnerEntryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}