using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(p => p.ProfileImageUrl)
                .HasMaxLength(255);

            builder.Property(p => p.SkillLevel)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(p => p.PrimaryCategory)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.HasIndex(p => p.UserId).IsUnique();

            // One-to-one: User <-> UserProfile
            builder.HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many UserProfiles -> one Province (don't cascade-delete profiles when a province is removed)
            builder.HasOne(p => p.Province)
                .WithMany()
                .HasForeignKey(p => p.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
