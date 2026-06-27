using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class AwardConfiguration : BaseEntityConfiguration<Award>
{
    public override void Configure(EntityTypeBuilder<Award> builder)
    {
        base.Configure(builder);

        builder.ToTable("awards");

        builder.Property(x => x.AwardName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Organization)
            .HasMaxLength(200);

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Awards)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
