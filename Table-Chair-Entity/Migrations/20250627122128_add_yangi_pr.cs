using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class add_yangi_pr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Models",
                table: "Slider",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Models",
                table: "Slider",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RedirectUrl",
                schema: "Models",
                table: "Slider",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Models",
                table: "Blog",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Models",
                table: "Blog",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Models",
                table: "AboutInfo",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "AboutInfo",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Models",
                table: "AboutInfo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Models",
                table: "AboutInfo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Models",
                table: "AboutInfo",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Models",
                table: "Slider");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Models",
                table: "Slider");

            migrationBuilder.DropColumn(
                name: "RedirectUrl",
                schema: "Models",
                table: "Slider");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Models",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Models",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Models",
                table: "AboutInfo");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "AboutInfo");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Models",
                table: "AboutInfo");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Models",
                table: "AboutInfo");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Models",
                table: "AboutInfo");
        }
    }
}
