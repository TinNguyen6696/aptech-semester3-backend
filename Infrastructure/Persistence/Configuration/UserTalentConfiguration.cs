using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Infrastructure.Persistence.Configuration;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence;

public class UserTalentConfiguration : BaseEntityConfiguration<UserTalent>
{
    public override void Configure(EntityTypeBuilder<UserTalent> builder)
    {
        base.Configure(builder);

        builder.ToTable("user_talents");

        builder.Property(x => x.Level)
            .HasMaxLength(50);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.TalentId
        }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserTalents)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Talent)
            .WithMany(x => x.UserTalents)
            .HasForeignKey(x => x.TalentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
