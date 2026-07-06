using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Category)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(v => v.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(v => v.VideoUrl)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(v => v.ThumbnailUrl)
                .HasMaxLength(255);

            builder.Property(v => v.Visibility)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.HasIndex(v => v.UserId);

            builder.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
