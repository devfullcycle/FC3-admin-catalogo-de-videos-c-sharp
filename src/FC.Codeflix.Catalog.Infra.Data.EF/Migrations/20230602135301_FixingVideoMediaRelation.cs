using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FC.Codeflix.Catalog.Infra.Data.EF.Migrations
{
    public partial class FixingVideoMediaRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Videos_Id",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Videos_VideoId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_VideoId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Media");

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "Videos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "TrailerId",
                table: "Videos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_MediaId",
                table: "Videos",
                column: "MediaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TrailerId",
                table: "Videos",
                column: "TrailerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Media_MediaId",
                table: "Videos",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Media_TrailerId",
                table: "Videos",
                column: "TrailerId",
                principalTable: "Media",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Media_MediaId",
                table: "Videos");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Media_TrailerId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_MediaId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_TrailerId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "TrailerId",
                table: "Videos");

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
                name: "FK_Media_Videos_Id",
                table: "Media",
                column: "Id",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Videos_VideoId",
                table: "Media",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id");
        }
    }
}
