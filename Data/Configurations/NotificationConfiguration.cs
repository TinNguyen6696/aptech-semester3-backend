using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.ReferenceType)
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(n => n.UserId);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}