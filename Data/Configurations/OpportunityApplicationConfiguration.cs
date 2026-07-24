using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class OpportunityApplicationConfiguration : IEntityTypeConfiguration<OpportunityApplication>
    {
        public void Configure(EntityTypeBuilder<OpportunityApplication> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasIndex(a => new { a.OpportunityId, a.ApplicantUserId }).IsUnique();

            builder.HasOne(a => a.Opportunity)
                .WithMany()
                .HasForeignKey(a => a.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict, not Cascade: Opportunity already cascades from User (PostedByUserId), so a
            // second direct cascade path from User into OpportunityApplication would trigger SQL
            // Server's multiple-cascade-paths error. Same fix pattern as Rating/VideoView/Follow/
            // Message/ContestVote/Report.
            builder.HasOne(a => a.ApplicantUser)
                .WithMany()
                .HasForeignKey(a => a.ApplicantUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
