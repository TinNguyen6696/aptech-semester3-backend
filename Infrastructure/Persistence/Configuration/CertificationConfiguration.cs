using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaLentShowcase.API.Models.Entities;

namespace TaLentShowcase.API.Infrastructure.Persistence.Configuration;

public class CertificationConfiguration : BaseEntityConfiguration<Certification>
{
    public override void Configure(EntityTypeBuilder<Certification> builder)
    {
        base.Configure(builder);

        builder.ToTable("certifications");

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IssuedBy)
            .HasMaxLength(200);

        builder.Property(x => x.CertificateUrl)
            .HasMaxLength(500);

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Certifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
