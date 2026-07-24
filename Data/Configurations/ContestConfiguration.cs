using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentShowcase.Api.Models.Entities;

namespace TalentShowcase.Api.Data.Configurations
{
    public class ContestConfiguration : IEntityTypeConfiguration<Contest>
    {
        public void Configure(EntityTypeBuilder<Contest> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.HasIndex(c => c.CreatedByUserId);

            builder.HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict, not Cascade/SetNull: ContestEntry.ContestId already cascades FROM Contest,
            // so a second auto-action (Cascade or SetNull) on this reverse WinnerEntryId FK would
            // create a cycle back to Contest — SQL Server rejects that at migration time. The
            // service layer clears WinnerEntryId before deleting a contest, so this never blocks
            // a real deletion.
            builder.HasOne(c => c.WinnerEntry)
                .WithMany()
                .HasForeignKey(c => c.WinnerEntryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}