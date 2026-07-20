using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowcase.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostedAtToOpportunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing rows predate this column — back-fill each one to its own CreatedAt
            // (the closest real value we have for "when it was posted") rather than a shared
            // constant. A DEFAULT constraint can't reference another column, so this needs the
            // add-nullable -> backfill -> tighten-to-not-null sequence instead of a single AddColumn.
            migrationBuilder.AddColumn<DateTime>(
                name: "PostedAt",
                table: "Opportunities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE Opportunities SET PostedAt = CreatedAt WHERE PostedAt IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostedAt",
                table: "Opportunities",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostedAt",
                table: "Opportunities");
        }
    }
}
