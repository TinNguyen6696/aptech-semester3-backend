using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Type)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Issuer)
                .HasMaxLength(150);

            builder.Property(a => a.CertificateUrl)
                .HasMaxLength(255);

            builder.Property(a => a.ExternalUrl)
                .HasMaxLength(500);

            builder.HasIndex(a => a.UserId);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
