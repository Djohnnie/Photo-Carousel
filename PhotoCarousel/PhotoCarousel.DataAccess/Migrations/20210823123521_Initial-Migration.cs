using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoCarousel.DataAccess.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PHOTOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sha256Hash = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: true),
                    SourcePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orientation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTaken = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHOTOS", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PHOTOS_SysId",
                table: "PHOTOS",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PHOTOS");
        }
    }
}