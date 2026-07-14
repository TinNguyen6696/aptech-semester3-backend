using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Data.Seeders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class CommunityConfiguration : IEntityTypeConfiguration<Community>
    {
        public void Configure(EntityTypeBuilder<Community> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            // Exactly one community per TalentCategory — the 6 are a fixed, permanent set.
            builder.HasIndex(c => c.Category).IsUnique();

            // Restrict, not Cascade: communities are permanent fixtures, never deleted via the
            // app, so there's no reason for a user-delete (which doesn't exist yet anyway) to
            // ever cascade into wiping them out.
            builder.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(CommunitySeeder.GetCommunities());
        }
    }
}