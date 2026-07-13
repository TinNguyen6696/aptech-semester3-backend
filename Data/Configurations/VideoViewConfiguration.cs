using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class VideoViewConfiguration : IEntityTypeConfiguration<VideoView>
    {
        public void Configure(EntityTypeBuilder<VideoView> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasIndex(v => new { v.VideoId, v.UserId })
                .IsUnique();

            builder.HasOne(v => v.Video)
                .WithMany()
                .HasForeignKey(v => v.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Same multiple-cascade-paths reasoning as RatingConfiguration: keep User FK Restrict.
            builder.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}