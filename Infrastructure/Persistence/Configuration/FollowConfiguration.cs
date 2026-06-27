using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class FollowConfiguration : BaseEntityConfiguration<Follow>
{
    public override void Configure(EntityTypeBuilder<Follow> builder)
    {
        base.Configure(builder);

        builder.ToTable("follows");

        builder.HasIndex(x => new
        {
            x.FollowerId,
            x.FollowingId
        }).IsUnique();

        builder.HasOne(x => x.Follower)
            .WithMany()
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Following)
            .WithMany()
            .HasForeignKey(x => x.FollowingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new
        {
            x.FollowerId,
            x.FollowingId
        }).IsUnique();

        builder.HasIndex(x => x.FollowingId);
    }
}
