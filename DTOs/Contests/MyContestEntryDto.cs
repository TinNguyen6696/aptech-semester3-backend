using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Contests
{
    // One of the current user's contest submissions, enriched with the contest it's in —
    // for the "My Entries" tab. The FE derives running-vs-ended from start/end vs. now.
    public class MyContestEntryDto
    {
        public int EntryId { get; set; }
        public int VoteCount { get; set; }
        public DateTime EnteredAt { get; set; }

        public int VideoId { get; set; }
        public string VideoTitle { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }

        public int ContestId { get; set; }
        public string ContestTitle { get; set; } = null!;
        public TalentCategory ContestCategory { get; set; }
        public DateTime ContestStartDate { get; set; }
        public DateTime ContestEndDate { get; set; }
    }
}
