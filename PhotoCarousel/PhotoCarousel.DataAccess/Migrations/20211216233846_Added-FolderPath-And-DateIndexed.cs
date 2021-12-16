using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoCarousel.DataAccess.Migrations
{
    public partial class AddedFolderPathAndDateIndexed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateIndexed",
                table: "PHOTOS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<string>(
                name: "FolderPath",
                table: "PHOTOS",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PHOTOS_FolderPath",
                table: "PHOTOS",
                column: "FolderPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PHOTOS_FolderPath",
                table: "PHOTOS");

            migrationBuilder.DropColumn(
                name: "DateIndexed",
                table: "PHOTOS");

            migrationBuilder.DropColumn(
                name: "FolderPath",
                table: "PHOTOS");
        }
    }
}