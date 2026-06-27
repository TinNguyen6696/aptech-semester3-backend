using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class RatingConfiguration : BaseEntityConfiguration<Rating>
{
    public override void Configure(EntityTypeBuilder<Rating> builder)
    {
        base.Configure(builder);

        builder.ToTable("ratings");

        builder.Property(x => x.Score)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.VideoId,
            x.UserId
        }).IsUnique();

        builder.HasOne(x => x.Video)
            .WithMany(x => x.Ratings)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Ratings)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new
        {
            x.VideoId,
            x.UserId
        }).IsUnique();
    }
}