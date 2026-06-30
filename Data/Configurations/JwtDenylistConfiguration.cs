using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class JwtDenylistConfiguration : IEntityTypeConfiguration<JwtDenylist>
    {
        public void Configure(EntityTypeBuilder<JwtDenylist> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Jti)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(j => j.Jti);
        }
    }
}
