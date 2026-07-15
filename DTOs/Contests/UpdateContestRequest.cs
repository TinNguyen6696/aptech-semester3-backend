using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Contests
{
    public class UpdateContestRequest
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public TalentCategory? Category { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }

        [Required]
        public DateTime? EndDate { get; set; }
    }
}