using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FC.Codeflix.Catalog.Infra.Data.EF.Migrations
{
    public partial class AddingTrailerToVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VideoId",
                table: "Media",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Media_VideoId",
                table: "Media",
                column: "VideoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Videos_VideoId",
                table: "Media",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Videos_VideoId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_VideoId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Media");
        }
    }
}
