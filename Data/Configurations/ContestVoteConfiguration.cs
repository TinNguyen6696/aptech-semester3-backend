using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class ContestVoteConfiguration : IEntityTypeConfiguration<ContestVote>
    {
        public void Configure(EntityTypeBuilder<ContestVote> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasIndex(v => new { v.ContestEntryId, v.UserId }).IsUnique();

            builder.HasOne(v => v.ContestEntry)
                .WithMany()
                .HasForeignKey(v => v.ContestEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict, not Cascade: avoids a multiple-cascade-paths error. User already
            // cascades to Contest, which cascades to ContestEntry, which cascades to
            // ContestVote — a second direct cascade path via UserId would conflict with that.
            builder.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}