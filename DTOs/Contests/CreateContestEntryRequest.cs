using System.ComponentModel.DataAnnotations;

namespace TalentShowcase.Api.DTOs.Contests
{
    public class CreateContestEntryRequest
    {
        [Required]
        public int? VideoId { get; set; }
    }
}