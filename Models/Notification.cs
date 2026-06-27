using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Models;

public class Notification : BaseEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public bool IsRead { get; set; }
}
