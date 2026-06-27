using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.JWT;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("refresh_tokens");

        builder.Property(x => x.Token)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Revoked)
            .HasDefaultValue(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Token);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}
