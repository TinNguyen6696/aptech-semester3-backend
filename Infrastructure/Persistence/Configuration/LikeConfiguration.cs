using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class LikeConfiguration : BaseEntityConfiguration<Like>
{
    public override void Configure(EntityTypeBuilder<Like> builder)
    {
        base.Configure(builder);

        builder.ToTable("likes");

        builder.HasIndex(x => new
        {
            x.VideoId,
            x.UserId
        }).IsUnique();

        builder.HasOne(x => x.Video)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new
        {
            x.VideoId,
            x.UserId
        }).IsUnique();
    }
}
