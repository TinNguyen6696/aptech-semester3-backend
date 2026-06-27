using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class TalentConfiguration : BaseEntityConfiguration<Talent>
{
    public override void Configure(EntityTypeBuilder<Talent> builder)
    {
        base.Configure(builder);

        builder.ToTable("talents");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
