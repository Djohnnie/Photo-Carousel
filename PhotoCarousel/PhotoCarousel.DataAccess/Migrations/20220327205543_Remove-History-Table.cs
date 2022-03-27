using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoCarousel.DataAccess.Migrations
{
    public partial class RemoveHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HISTORY");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HISTORY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scheduled = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORY", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HISTORY_Scheduled",
                table: "HISTORY",
                column: "Scheduled");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORY_SysId",
                table: "HISTORY",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }
    }
}