using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowcase.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRemovedByAdminToVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemovedByAdmin",
                table: "Videos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemovedByAdmin",
                table: "Videos");
        }
    }
}
