namespace TaLentShowcase.API.Models.Entities;

public class Message : BaseEntity
{
    public int SenderId { get; set; }

    public User Sender { get; set; } = null!;

    public int ReceiverId { get; set; }

    public User Receiver { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsRead { get; set; }
}
