using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ContosoUniversityCore.Migrations
{
    public partial class UpdatePictureUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalUrl",
                table: "Picture",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Picture",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoragePath",
                table: "Picture",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Picture",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalUrl",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "StoragePath",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Picture");
        }
    }
}
