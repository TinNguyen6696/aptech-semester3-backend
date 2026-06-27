using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class CommunityConfiguration : BaseEntityConfiguration<Community>
{
    public override void Configure(EntityTypeBuilder<Community> builder)
    {
        base.Configure(builder);

        builder.ToTable("communities");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.TalentId);

        builder.HasOne(x => x.Talent)
            .WithMany(x => x.Communities)
            .HasForeignKey(x => x.TalentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
