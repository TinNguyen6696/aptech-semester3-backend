using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class VideoConfiguration : BaseEntityConfiguration<Video>
{
    public override void Configure(EntityTypeBuilder<Video> builder)
    {
        base.Configure(builder);

        builder.ToTable("videos");

        builder.Property(x => x.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.VideoUrl)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(255);

        builder.HasIndex(x => x.TalentId);

        builder.HasOne(x => x.Talent)
            .WithMany(x => x.Videos)
            .HasForeignKey(x => x.TalentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Visibility)
            .HasConversion<string>();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Videos)
            .HasForeignKey(x => x.UserId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.Visibility);

        builder.HasIndex(x => x.CreatedAt);
    }
}
