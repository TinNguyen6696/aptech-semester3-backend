namespace TaLentShowcase.API.Models.Entities;

public class Province : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}
