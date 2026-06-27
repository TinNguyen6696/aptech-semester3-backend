using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.JWT;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class JwtDenylistConfiguration : BaseEntityConfiguration<JwtDenylist>
{
    public override void Configure(EntityTypeBuilder<JwtDenylist> builder)
    {
        base.Configure(builder);

        builder.ToTable("jwt_denylists");

        builder.Property(x => x.Jti)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.Jti)
            .IsUnique();

        builder.HasIndex(x => x.Jti)
            .IsUnique();
    }
}