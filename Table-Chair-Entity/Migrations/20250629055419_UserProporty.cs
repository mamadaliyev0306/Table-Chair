using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Table_Chair_Entity.Migrations
{
    /// <inheritdoc />
    public partial class UserProporty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                schema: "Models",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneVerificationToken",
                schema: "Models",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhoneVerificationTokenExpires",
                schema: "Models",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "AboutInfo",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Models",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                schema: "Models",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhoneVerificationToken",
                schema: "Models",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhoneVerificationTokenExpires",
                schema: "Models",
                table: "User");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                schema: "Models",
                table: "AboutInfo",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
