using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class ContestConfiguration : IEntityTypeConfiguration<Contest>
    {
        public void Configure(EntityTypeBuilder<Contest> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.HasIndex(c => c.CreatedByUserId);

            builder.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}