using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class ContestEntryConfiguration : BaseEntityConfiguration<ContestEntry>
{
    public override void Configure(EntityTypeBuilder<ContestEntry> builder)
    {
        base.Configure(builder);

        builder.ToTable("contest_entries");

        builder.HasIndex(x => new
        {
            x.ContestId,
            x.VideoId
        }).IsUnique();

        builder.HasOne(x => x.Contest)
            .WithMany(x => x.Entries)
            .HasForeignKey(x => x.ContestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Video)
            .WithMany(x => x.ContestEntries)
            .HasForeignKey(x => x.VideoId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
