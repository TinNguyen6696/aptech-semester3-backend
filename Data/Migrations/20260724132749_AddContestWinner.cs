using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentShowcase.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContestWinner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WinnerAnnouncedAt",
                table: "Contests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WinnerEntryId",
                table: "Contests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contests_WinnerEntryId",
                table: "Contests",
                column: "WinnerEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contests_ContestEntries_WinnerEntryId",
                table: "Contests",
                column: "WinnerEntryId",
                principalTable: "ContestEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contests_ContestEntries_WinnerEntryId",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Contests_WinnerEntryId",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "WinnerAnnouncedAt",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "WinnerEntryId",
                table: "Contests");
        }
    }
}
