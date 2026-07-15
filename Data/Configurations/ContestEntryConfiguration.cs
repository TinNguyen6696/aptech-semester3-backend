using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class ContestEntryConfiguration : IEntityTypeConfiguration<ContestEntry>
    {
        public void Configure(EntityTypeBuilder<ContestEntry> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.ContestId, e.VideoId }).IsUnique();

            builder.HasOne(e => e.Contest)
                .WithMany()
                .HasForeignKey(e => e.ContestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict, not Cascade: a video that has ever been entered into a contest must
            // never be deletable (preserves contest history/vote integrity). This FK is a
            // DB-level safety net alongside the permanent service-level block in VideoService.
            builder.HasOne(e => e.Video)
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}