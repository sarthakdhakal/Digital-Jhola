using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    public partial class ratingedit1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StarRatings_AspNetUsers_ReviewerId",
                table: "StarRatings");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                table: "StarRatings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StarRatings_ReviewerId",
                table: "StarRatings",
                newName: "IX_StarRatings_UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "StarRatings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StarRatings_AspNetUsers_UserId",
                table: "StarRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StarRatings_AspNetUsers_UserId",
                table: "StarRatings");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "StarRatings");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StarRatings",
                newName: "ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_StarRatings_UserId",
                table: "StarRatings",
                newName: "IX_StarRatings_ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StarRatings_AspNetUsers_ReviewerId",
                table: "StarRatings",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
