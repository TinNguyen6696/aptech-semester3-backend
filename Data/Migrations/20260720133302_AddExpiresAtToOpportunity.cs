using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowcase.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiresAtToOpportunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing rows predate this column and have no real expiry — back-fill them to
            // 30 days from the moment this migration runs, so they don't show as expired on day one.
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Opportunities",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "DATEADD(day, 30, GETUTCDATE())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Opportunities");
        }
    }
}
