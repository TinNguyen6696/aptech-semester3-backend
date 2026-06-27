namespace TaLentShowcase.API.DTOS.Profile;

public class UpsertCertificationRequest
{
    public string Name { get; set; } = string.Empty;

    public string? IssuedBy { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string? CertificateUrl { get; set; }
}
