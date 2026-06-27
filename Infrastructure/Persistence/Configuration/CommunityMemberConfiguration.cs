using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class CommunityMemberConfiguration : BaseEntityConfiguration<CommunityMember>
{
    public override void Configure(EntityTypeBuilder<CommunityMember> builder)
    {
        base.Configure(builder);

        builder.ToTable("community_members");

        builder.HasOne(x => x.Community)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.CommunityId,
            x.UserId
        }).IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}
