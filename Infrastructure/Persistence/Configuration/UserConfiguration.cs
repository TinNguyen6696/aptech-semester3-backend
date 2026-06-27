using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("users");

        builder.Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ProfileImageUrl)
            .HasMaxLength(255);

        builder.Property(x => x.Role)
            .HasConversion<string>();

        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.ProvinceId);

        builder.HasOne(x => x.Province)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
