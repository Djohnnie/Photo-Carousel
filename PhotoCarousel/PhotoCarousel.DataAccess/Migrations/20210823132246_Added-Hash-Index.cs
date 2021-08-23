using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoCarousel.DataAccess.Migrations
{
    public partial class AddedHashIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PHOTOS_Sha256Hash",
                table: "PHOTOS",
                column: "Sha256Hash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PHOTOS_Sha256Hash",
                table: "PHOTOS");
        }
    }
}