using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class VideoViewConfiguration : BaseEntityConfiguration<VideoView>
{
    public override void Configure(EntityTypeBuilder<VideoView> builder)
    {
        base.Configure(builder);
    
        builder.ToTable("video_views");

        builder.HasOne(x => x.Video)
            .WithMany(x => x.VideoViews)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.VideoId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.CreatedAt);
    }
}
