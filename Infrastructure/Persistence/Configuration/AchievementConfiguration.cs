using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class AchievementConfiguration : BaseEntityConfiguration<Achievement>
{
    public override void Configure(EntityTypeBuilder<Achievement> builder)
    {
        base.Configure(builder);

        builder.ToTable("achievements");

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Achievements)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
