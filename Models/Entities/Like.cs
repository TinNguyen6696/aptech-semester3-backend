namespace TaLentShowcase.API.Models.Entities;

public class Like : BaseEntity
{
    public int VideoId { get; set; }

    public Video Video { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
