using Microsoft.EntityFrameworkCore;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Province> Provinces => Set<Province>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<JwtDenylist> JwtDenylists => Set<JwtDenylist>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<Video> Videos => Set<Video>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Rating> Ratings => Set<Rating>();
        public DbSet<VideoView> VideoViews => Set<VideoView>();
        public DbSet<Follow> Follows => Set<Follow>();
        public DbSet<Community> Communities => Set<Community>();
        public DbSet<CommunityPost> CommunityPosts => Set<CommunityPost>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Opportunity> Opportunities => Set<Opportunity>();
        public DbSet<Contest> Contests => Set<Contest>();
        public DbSet<ContestEntry> ContestEntries => Set<ContestEntry>();
        public DbSet<ContestVote> ContestVotes => Set<ContestVote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyTimestamps()
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
        }
    }
}
