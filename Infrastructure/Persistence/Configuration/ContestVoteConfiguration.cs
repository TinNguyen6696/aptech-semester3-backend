using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class ContestVoteConfiguration : BaseEntityConfiguration<ContestVote>
{
    public override void Configure(EntityTypeBuilder<ContestVote> builder)
    {
        base.Configure(builder);

        builder.ToTable("contest_votes");

        builder.HasIndex(x => new
        {
            x.ContestEntryId,
            x.UserId
        }).IsUnique();

        builder.HasOne(x => x.ContestEntry)
            .WithMany(x => x.Votes)
            .HasForeignKey(x => x.ContestEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new
        {
            x.ContestEntryId,
            x.UserId
        }).IsUnique();
    }
}
