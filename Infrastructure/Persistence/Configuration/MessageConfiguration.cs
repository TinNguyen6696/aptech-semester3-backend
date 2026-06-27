using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class MessageConfiguration : BaseEntityConfiguration<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.ToTable("messages");

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.ReceiverId);

        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Receiver)
            .WithMany()
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ReceiverId);

        builder.HasIndex(x => x.SenderId);

        builder.HasIndex(x => x.CreatedAt);
    }
}
