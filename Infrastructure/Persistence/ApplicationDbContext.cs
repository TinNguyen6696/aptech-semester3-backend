using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TaLentShowcase.API.Models;
using TaLentShowcase.API.Models.Entities;
using TaLentShowcase.API.Models.JWT;

namespace TaLentShowcase.API.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<VideoView> VideoViews => Set<VideoView>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Community> Communities => Set<Community>();
    public DbSet<CommunityMember> CommunityMembers => Set<CommunityMember>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Contest> Contests => Set<Contest>();
    public DbSet<ContestEntry> ContestEntries => Set<ContestEntry>();
    public DbSet<ContestVote> ContestVotes => Set<ContestVote>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<JwtDenylist> JwtDenylists => Set<JwtDenylist>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<Award> Awards => Set<Award>();
    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<Talent> Talents => Set<Talent>();
    public DbSet<UserTalent> UserTalents => Set<UserTalent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
