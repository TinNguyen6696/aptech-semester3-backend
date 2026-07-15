using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
    {
        public void Configure(EntityTypeBuilder<Opportunity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Category)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(o => o.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.Description)
                .IsRequired();

            builder.HasIndex(o => o.PostedByUserId);
            builder.HasIndex(o => o.ProvinceId);

            builder.HasOne(o => o.PostedByUser)
                .WithMany()
                .HasForeignKey(o => o.PostedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Same reasoning as UserProfile -> Province: don't cascade-delete opportunities
            // if a province were ever removed.
            builder.HasOne(o => o.Province)
                .WithMany()
                .HasForeignKey(o => o.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}