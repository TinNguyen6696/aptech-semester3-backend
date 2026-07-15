using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Opportunities
{
    public class CreateOpportunityRequest
    {
        [Required]
        public TalentCategory? Category { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public int? ProvinceId { get; set; }
    }
}