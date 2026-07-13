using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowcase.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalUrlToAchievement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "Achievements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "Achievements");
        }
    }
}
