using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class ContestConfiguration : BaseEntityConfiguration<Contest>
{
    public override void Configure(EntityTypeBuilder<Contest> builder)
    {
        base.Configure(builder);

        builder.ToTable("contests");

        builder.Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.HasIndex(x => x.TalentId);

        builder.HasOne(x => x.Talent)
            .WithMany(x => x.Contests)
            .HasForeignKey(x => x.TalentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.StartDate);

        builder.HasIndex(x => x.EndDate);
    }
}
