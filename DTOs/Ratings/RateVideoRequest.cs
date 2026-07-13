using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Ratings
{
    public class RateVideoRequest
    {
        [Required]
        public int? Score { get; set; }
    }
}