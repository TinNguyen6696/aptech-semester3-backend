namespace TaLentShowcase.API.Models.Entities;

public class Talent : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<UserTalent> UserTalents { get; set; } = new List<UserTalent>();

    public ICollection<Video> Videos { get; set; } = new List<Video>();

    public ICollection<Community> Communities { get; set; } = new List<Community>();

    public ICollection<Contest> Contests { get; set; } = new List<Contest>();

    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}