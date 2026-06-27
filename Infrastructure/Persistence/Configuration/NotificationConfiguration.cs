using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.ToTable("notifications");

        builder.Property(x => x.Content)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ReferenceType)
            .HasMaxLength(50);

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.IsRead);

        builder.HasIndex(x => x.CreatedAt);
    }
}