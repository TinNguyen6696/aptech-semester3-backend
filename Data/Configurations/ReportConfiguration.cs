using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.HasIndex(r => new { r.VideoId, r.ReporterUserId }).IsUnique();
            builder.HasIndex(r => r.Status);

            builder.HasOne(r => r.Video)
                .WithMany()
                .HasForeignKey(r => r.VideoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict, not Cascade: Video already cascades from User, so a second direct
            // cascade path from User into Report would trigger SQL Server's multiple-cascade-
            // paths error. Same fix pattern as Rating/VideoView/Follow/Message/ContestVote.
            builder.HasOne(r => r.ReporterUser)
                .WithMany()
                .HasForeignKey(r => r.ReporterUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReviewedByUser)
                .WithMany()
                .HasForeignKey(r => r.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}