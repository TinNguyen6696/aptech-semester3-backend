using System.ComponentModel.DataAnnotations;

namespace TaLentShowcase.API.DTOS.Profile;

public class UpsertCertificationRequest : IValidatableObject
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "IssuedBy cannot exceed 200 characters.")]
    public string? IssuedBy { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    [StringLength(500, ErrorMessage = "CertificateUrl cannot exceed 500 characters.")]
    [Url(ErrorMessage = "CertificateUrl must be a valid URL.")]
    public string? CertificateUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IssueDate.HasValue && ExpiredDate.HasValue && ExpiredDate < IssueDate)
        {
            yield return new ValidationResult(
                "ExpiredDate must be on or after IssueDate.",
                new[] { nameof(ExpiredDate) });
        }
    }
}
