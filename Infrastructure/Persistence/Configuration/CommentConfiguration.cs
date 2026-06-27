using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class CommentConfiguration : BaseEntityConfiguration<Comment>
{
    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        base.Configure(builder);

        builder.ToTable("comments");

        builder.Property(x => x.Content)
            .IsRequired();

        builder.HasOne(x => x.Video)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.VideoId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.CreatedAt);
    }
}
