using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoCarousel.DataAccess.Migrations
{
    public partial class AddedSourcePathIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SourcePath",
                table: "PHOTOS",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PHOTOS_Sha256Hash_SourcePath",
                table: "PHOTOS",
                columns: new[] { "Sha256Hash", "SourcePath" });

            migrationBuilder.CreateIndex(
                name: "IX_PHOTOS_SourcePath",
                table: "PHOTOS",
                column: "SourcePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PHOTOS_Sha256Hash_SourcePath",
                table: "PHOTOS");

            migrationBuilder.DropIndex(
                name: "IX_PHOTOS_SourcePath",
                table: "PHOTOS");

            migrationBuilder.AlterColumn<string>(
                name: "SourcePath",
                table: "PHOTOS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
