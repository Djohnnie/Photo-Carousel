using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoCarousel.DataAccess.Migrations
{
    public partial class AddHistoryTableWithValueGeneratedOnAddForSysId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HISTORY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scheduled = table.Column<DateTime>(type: "datetime2", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HISTORY");
        }
    }
}