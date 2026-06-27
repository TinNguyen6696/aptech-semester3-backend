using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class OpportunityConfiguration : BaseEntityConfiguration<Opportunity>
{
    public override void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        base.Configure(builder);

        builder.ToTable("opportunities");

        builder.Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.HasIndex(x => x.TalentId);

        builder.HasOne(x => x.Talent)
            .WithMany()
            .HasForeignKey(x => x.TalentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PostedByUser)
            .WithMany()
            .HasForeignKey(x => x.PostedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Talent)
            .WithMany(x => x.Opportunities)
            .HasForeignKey(x => x.TalentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProvinceId);

        builder.HasIndex(x => x.PostedByUserId);
    }
}
