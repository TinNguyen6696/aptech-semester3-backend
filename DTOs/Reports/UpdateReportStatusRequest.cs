using System.ComponentModel.DataAnnotations;
using TalentShowcase.Api.Models.Enums;

namespace TalentShowcase.Api.DTOs.Reports
{
    public class UpdateReportStatusRequest
    {
        [Required]
        public ReportStatus? Status { get; set; }
    }
}