using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesignStudio.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cient_reviews",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Client",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "Projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Passage",
                table: "Projects",
                newName: "Address");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Projects",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Projects",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Projects",
                newName: "Passage");

            migrationBuilder.AddColumn<string>(
                name: "Cient_reviews",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Client",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
